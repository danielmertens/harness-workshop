#:package GitHub.Copilot.SDK@1.0.34
#:property PublishAot=false
#pragma warning disable GHCP001   // typed replies (SendAndWaitAsync<T>) are experimental in the SDK

// The development loop: plan -> implement -> quality gate -> review -> commit.
// Run it from this folder:   dotnet run loop.cs .workitems/T01/task.md   (the task file, relative to the repository)
//
// This file is generic. Everything that decides HOW the work is done lives in the repository:
//   .workitems/<id>/task.md    what to build (written by a human)
//   .github/skills/*/SKILL.md  how each step works (plan, implement, review)
//   quality-gate.ps1           what "green" means
//   .github/hooks/             which tool calls are allowed (guardrails)
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitHub.Copilot;

const string Model = "claude-sonnet-5.5";
const int MaxAttempts = 3;
var Json = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

var task = args[0];                           // the task file, relative to the repository: .workitems/T01/task.md
var workItem = Path.GetDirectoryName(task)!.Replace('\\', '/');  // its folder: every step writes its output next to the task
var id = Path.GetFileName(workItem);          // a short name, for the branch and the commit message

// Menu A: every work item gets its own branch in its own worktree. Main is never touched,
// and running the same work item again continues where it stopped.
var repo = Path.GetFullPath($"worktrees/{id}");
Console.WriteLine($"Work item {id} in {repo}");
if (!Directory.Exists(repo))
{
    Console.WriteLine($"Creating worktree on branch workitem/{id}...");
    Run("git", $"worktree add \"{repo}\" -b workitem/{id}", Path.GetFullPath("app"));
}
var workItemDir = Path.Combine(repo, workItem);

if (!File.Exists(Path.Combine(workItemDir, "plan.md"))) await Skill("plan", $"/plan {task}");
else Console.WriteLine($"{"plan",-12} skipped, {workItem}/plan.md already exists");

string? feedback = null;
for (var attempt = 1; attempt <= MaxAttempts; attempt++)
{
    Console.WriteLine($"--- attempt {attempt} of {MaxAttempts}");
    await Skill($"implement-{attempt}", $"/implement {workItem} {feedback}".Trim());

    feedback = QualityGate(attempt);
    if (feedback is not null) continue;

    feedback = await Review(attempt);
    if (feedback is not null) continue;

    Run("git", "add -A");
    Run("git", $"commit -m \"{id}: implemented by the development loop\"");
    Console.WriteLine($"DONE     {id} is committed on branch workitem/{id}.");
    Console.WriteLine($"         Next: push the branch and open a merge request, where a human reviews it.");
    return 0;
}

Console.WriteLine($"STOPPED  {id} still has problems after {MaxAttempts} attempts. A human takes over: see {workItem}/.");
return 1;

// One step = one skill, run in a fresh agent session. Returns the agent's final answer.
async Task<string> Skill(string step, string prompt)
{
    Console.WriteLine($"{step,-12} started");
    var timer = Stopwatch.StartNew();
    await using var client = await NewClient(step);
    await using var session = await NewSession(client, step);
    var reply = await session.SendAndWaitAsync(prompt, TimeSpan.FromMinutes(30));
    Console.WriteLine($"{step,-12} done in {timer.Elapsed:m\\:ss}");
    return reply?.Data.Content ?? "";
}

// The repository's own quality gate decides. Returns null when green, otherwise the file with instructions for the agent.
string? QualityGate(int attempt)
{
    Console.WriteLine($"{"gate",-12} running quality-gate.ps1...");
    var (exitCode, output) = Run("pwsh", "-NoProfile -File quality-gate.ps1", check: false);
    var file = $"{workItem}/gate-{attempt}.md";
    File.WriteAllText(Path.Combine(repo, file), output);
    Console.WriteLine($"{"gate",-12} {(exitCode == 0 ? "passed" : "FAILED")}  {file}");
    return exitCode == 0 ? null : file;
}

// Agentic review: the review skill writes the review, then a follow-up in the SAME session turns it into data.
// The harness only owns the JSON shape (the records at the bottom). What goes into it is defined by the review skill.
// Returns null when there is nothing high or medium to fix, otherwise the file with the findings.
async Task<string?> Review(int attempt)
{
    Console.WriteLine($"{"review",-12} started");
    await using var client = await NewClient($"review-{attempt}");
    await using var session = await NewSession(client, $"review-{attempt}");
    var review = await session.SendAndWaitAsync($"/review {workItem}", TimeSpan.FromMinutes(30));
    File.WriteAllText(Path.Combine(workItemDir, $"review-{attempt}.md"), review?.Data.Content);

    var result = await session.SendAndWaitAsync<ReviewResult>("Your findings as JSON.", timeout: TimeSpan.FromMinutes(5));

    var file = $"{workItem}/review-{attempt}.json";
    File.WriteAllText(Path.Combine(repo, file), JsonSerializer.Serialize(result, Json));
    var blocking = result!.Findings.Count(f => f.Severity is Severity.High or Severity.Medium);
    Console.WriteLine($"{"review",-12} {result.Findings.Count} findings, {blocking} high or medium  {file}");
    return blocking > 0 ? file : null;
}

// Menu C: one Copilot runtime per step, so every step gets its own OpenTelemetry file: <step>.otel.jsonl.
// Telemetry is configured per runtime, not per session, and the file is complete once the client is disposed.
async Task<CopilotClient> NewClient(string step)
{
    var client = new CopilotClient(new CopilotClientOptions
    {
        Telemetry = new TelemetryConfig { ExporterType = "file", FilePath = Path.Combine(workItemDir, $"{step}.otel.jsonl") },
    });
    await client.StartAsync();
    return client;
}

async Task<CopilotSession> NewSession(CopilotClient client, string step)
{
    var log = Path.Combine(workItemDir, $"{step}.events.jsonl");
    var gate = new object();
    return await client.CreateSessionAsync(new SessionConfig
    {
        Model = Model,
        WorkingDirectory = repo,
        TrustWorkingDirectory = WorkingDirectoryTrust.Session,
        EnableConfigDiscovery = true,                 // loads the skills in .github/skills
        OnPermissionRequest = PermissionHandler.ApproveAll,   // the guardrails are hooks in the repository (menu B)
        OnEvent = e =>
        {
            lock (gate) File.AppendAllText(log, e.ToJson() + "\n");   // Lab 6: every event of this step
            if (e is ToolExecutionStartEvent t) Console.WriteLine($"  {step,-12} {t.Data.ToolName}");
            if (e is ToolExecutionCompleteEvent { Data.Success: false } c) Console.WriteLine($"  {step,-12} FAILED {c.Data.Error?.Message}");
        },
    });
}

// Runs a command (in the repository unless told otherwise) and returns its exit code and output.
(int ExitCode, string Output) Run(string command, string arguments, string? dir = null, bool check = true)
{
    var p = Process.Start(new ProcessStartInfo(command, arguments)
        { WorkingDirectory = dir ?? repo, RedirectStandardOutput = true, RedirectStandardError = true })!;
    var errors = p.StandardError.ReadToEndAsync();
    var output = p.StandardOutput.ReadToEnd() + errors.Result;
    p.WaitForExit();
    if (check && p.ExitCode != 0) throw new Exception($"Failed: {command} {arguments}\n{output}");
    return (p.ExitCode, output);
}

record ReviewResult(List<Finding> Findings);
record Finding(Severity Severity, string Summary);
[JsonConverter(typeof(JsonStringEnumConverter<Severity>))] enum Severity { High, Medium, Low }
