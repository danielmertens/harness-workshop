# Harness engineering workshop: starter

Follow the labs in `workshop.html`. This folder is where you work.

| Path | What it is |
|---|---|
| `loop.cs` | The development loop: about 55 lines of C# that drive GitHub Copilot through the Copilot SDK. You grow it today. |
| `app/` | The repository the agent works in: `payroll`, a one-page Blazor app that shows a monthly payslip from an HR register. Start it with `dotnet run --project app/src/Payroll` and open <http://localhost:5080>. |
| `app/AGENTS.md` | Instructions for the agent: a code map, the build and test commands, and the rules for changes. The app's own `README.md` describes the app and its payslip rules. |
| `app/.workitems/` | One folder per work item. `task.md` says what to build. Every step of the loop writes its output here. |
| `app/.github/skills/` | The skills the loop calls. Empty at the start: you write `plan` and `implement` in Lab 2, and `review` in Lab 5. |

You need: .NET SDK 10 or later, git, PowerShell 7 (`pwsh`), and a GitHub account with Copilot.

First time:

```
cd app
git init -b main
git add -A
git commit -m "baseline"
cd ..
dotnet run loop.cs T01
```

The first `dotnet run` downloads the Copilot SDK and its Copilot runtime, which takes a minute.

Fell behind? `../snapshots/lab-N/` holds this folder as it is at the end of Lab N. See `../snapshots/README.md`.
