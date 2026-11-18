#:package GitHub.Copilot.SDK@1.0.34
#:property PublishAot=false

// The loop after Lab 2: every step is a skill. Plan, implement, commit.
// Run it from this folder:   dotnet run loop.cs .workitems/T01/task.md
using System.Diagnostics;
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
await Ask("implement", $"/implement {workItem}");

Console.WriteLine("Committing...");
Run("git", "add -A");
Run("git", $"commit -m \"{id}\"");
Console.WriteLine($"Done: {id} is committed.");

// One step = one prompt in a fresh agent session.
async Task Ask(string step, string prompt)
{
    Console.WriteLine($"--- {step}: started, the agent is working (this can take a few minutes)");
    var timer = Stopwatch.StartNew();
    await using var session = await NewSession();
    var reply = await session.SendAndWaitAsync(prompt, TimeSpan.FromMinutes(30));
    Console.WriteLine($"--- {step}: finished in {timer.Elapsed:m\\:ss}");
    Console.WriteLine(reply?.Data.Content);
}

async Task<CopilotSession> NewSession() => await client.CreateSessionAsync(new SessionConfig
{
    Model = "claude-sonnet-5.5",
    WorkingDirectory = repo,
    TrustWorkingDirectory = WorkingDirectoryTrust.Session,
    EnableConfigDiscovery = true,                         // loads the skills in app/.github/skills
    OnPermissionRequest = PermissionHandler.ApproveAll,   // the agent may do anything (for now)
    OnEvent = e => { if (e is ToolExecutionStartEvent t) Console.WriteLine($"  tool: {t.Data.ToolName}"); },
});

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
