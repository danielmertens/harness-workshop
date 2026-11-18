# Harness engineering workshop: snapshots

The starter folder (`loop.cs` and `app/`) as it is at the end of each lab, with the code exactly as it is on the
workshop page. Use them to catch up when you fall behind.

| Folder | State |
|---|---|
| `lab-1/` | The starter as shipped. Lab 1 only runs the loop and undoes the run. |
| `lab-2/` | Plan and implement skills. `loop.cs` calls `/plan` and `/implement` and takes the task file as its argument. |
| `lab-3/` | A simple `quality-gate.ps1` (build and test). `loop.cs` runs it and stops before the commit when it fails. |
| `lab-4/` | The gate prints instructions for the agent. The implement skill takes a feedback file. `loop.cs` retries up to three times. |
| `lab-5/` | The review skill. `loop.cs` reviews after a green gate and asks for the findings as JSON. |
| `lab-6/` | `loop.cs` writes every event of every step to `<step>.events.jsonl` in the work item folder. |
| `lab-7/` | The improve skill, for an interactive session. `loop.cs` is the same as in `lab-6/`. |

## Catch up

To start Lab N without finishing the lab before it: undo any run of the loop first, then, from your starter folder,
in a `pwsh` terminal, copy the snapshot of the lab before over it and commit it. For example, to start Lab 4:

```
Copy-Item ../snapshots/lab-3/* . -Recurse -Force
git -C app add -A
git -C app commit -m "Catch up to the end of Lab 3"
```

This overwrites your `loop.cs`, skills and gate with the snapshot's versions.

The menu items (A to D) are not in the snapshots. `../solution/` has the finished loop with menu A and C applied.
