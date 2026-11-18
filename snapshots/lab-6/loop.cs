#:package GitHub.Copilot.SDK@1.0.34
#:property PublishAot=false
#pragma warning disable GHCP001   // typed replies are experimental in the SDK

// The loop after Lab 6 (every step logs its events): plan, then implement, quality gate and review up to three times, commit.
// Run it from this folder:   dotnet run loop.cs .workitems/T01/task.md
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitHub.Copilot;

var task = args[0];                           // the task file, relative to the repository: .workitems/T01/task.md
var workItem = Path.GetDirectoryName(task)!.Replace('\\', '/');  // its folder: every step writes its output next to the task
var id = Path.GetFileName(workItem);          // a short name, for the commit message
var repo = Path.GetFullPath("app");        // the repository the agent works in
Console.WriteLine($"Work item {id} in {repo}");

Console.WriteLine("Starting the Copilot runtime...");
await using var client = new CopilotClient();
await client.StartAsync();

await Ask("plan", $"/plan {task}");
string? feedback = null;
for (var attempt = 1; attempt <= 3; attempt++)
{
    await Ask($"implement-{attempt}", $"/implement {workItem} {feedback}".Trim());

    feedback = QualityGate(attempt);
    if (feedback is not null) continue;

    feedback = await Review(attempt);
    if (feedback is not null) continue;

    Run("git", "add -A");
    Run("git", $"commit -m \"{id}\"");
    Console.WriteLine($"Done: {id} is committed.");
    return;
}
Console.WriteLine($"STOPPED: {id} still has problems after 3 attempts. A human takes over.");

// Runs the repository's quality gate. Returns null when green, otherwise the file with the instructions for the agent.
string? QualityGate(int attempt)
{
    var (exitCode, output) = Run("pwsh", "-NoProfile -File quality-gate.ps1", check: false);
    var file = $"{workItem}/gate-{attempt}.md";
    File.WriteAllText(Path.Combine(repo, file), output);
    Console.WriteLine($"Quality gate: {(exitCode == 0 ? "passed" : "FAILED")}, see {file}");
    return exitCode == 0 ? null : file;
}

// The review skill writes the review. A follow-up in the SAME session turns it into data.
async Task<string?> Review(int attempt)
{
    Console.WriteLine($"--- review-{attempt}: started");
    var timer = Stopwatch.StartNew();
    await using var session = await NewSession("review-" + attempt);
    var review = await session.SendAndWaitAsync($"/review {workItem}", TimeSpan.FromMinutes(30));
    File.WriteAllText(Path.Combine(repo, workItem, $"review-{attempt}.md"), review?.Data.Content);

    var result = await session.SendAndWaitAsync<ReviewResult>("Your findings as JSON.", timeout: TimeSpan.FromMinutes(5));

    var file = $"{workItem}/review-{attempt}.json";
    var json = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    File.WriteAllText(Path.Combine(repo, file), JsonSerializer.Serialize(result, json));
    var blocking = result!.Findings.Count(f => f.Severity is Severity.High or Severity.Medium);
    Console.WriteLine($"--- review-{attempt}: finished in {timer.Elapsed:m\\:ss}, {result.Findings.Count} findings, {blocking} high or medium, see {file}");
    return blocking > 0 ? file : null;
}

// One step = one prompt in a fresh agent session.
async Task Ask(string step, string prompt)
{
    Console.WriteLine($"--- {step}: started, the agent is working (this can take a few minutes)");
    var timer = Stopwatch.StartNew();
    await using var session = await NewSession(step);
    var reply = await session.SendAndWaitAsync(prompt, TimeSpan.FromMinutes(30));
    Console.WriteLine($"--- {step}: finished in {timer.Elapsed:m\\:ss}");
    Console.WriteLine(reply?.Data.Content);
}

async Task<CopilotSession> NewSession(string step)
{
    var log = Path.Combine(repo, workItem, $"{step}.events.jsonl");
    var gate = new object();
    return await client.CreateSessionAsync(new SessionConfig
    {
        Model = "claude-sonnet-5.5",
        WorkingDirectory = repo,
        TrustWorkingDirectory = WorkingDirectoryTrust.Session,
        EnableConfigDiscovery = true,                         // loads the skills in app/.github/skills
        OnPermissionRequest = PermissionHandler.ApproveAll,   // the agent may do anything (for now)
        OnEvent = e =>
        {
            lock (gate) File.AppendAllText(log, e.ToJson() + "\n");
            if (e is ToolExecutionStartEvent t) Console.WriteLine($"  {step}: {t.Data.ToolName}");
            if (e is ToolExecutionCompleteEvent { Data.Success: false } c) Console.WriteLine($"  {step}: FAILED {c.Data.Error?.Message}");
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
