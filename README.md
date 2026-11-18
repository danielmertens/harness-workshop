# Harness Engineering Workshop

A hands-on workshop in which you grow a naive agent loop into a harness: skills, a quality gate, retries, an
agentic review and event logs. The loop is a small C# program (a .NET 10 file-based app) that drives GitHub
Copilot through the Copilot SDK.

## Get started

1. Clone this repository.
2. Open `workshop.html` in your browser and follow the labs.
3. You work in `starter/`. Its `README.md` explains the first run.

You need the .NET SDK 10 or later, git, PowerShell 7 (`pwsh`) and a GitHub account with Copilot.

## What is in here

| Path | What it is |
|---|---|
| `workshop.html` | The workshop page: the labs, the menu, and a reference. |
| `slides.html` | The slides used between the labs. |
| `starter/` | Where you work: the loop (`loop.cs`) and the app the agent works in (`app/`). |
| `snapshots/` | The starter folder at the end of each lab. Fell behind? Copy the one you need (see its `README.md`). |
| `solution/` | The finished loop and skills, to compare with your own. |
| `nuget.config` | Restores NuGet packages from nuget.org only, so other feeds on your machine do not get in the way. |
