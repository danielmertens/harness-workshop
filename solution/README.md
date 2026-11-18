# Harness engineering workshop: solution

The finished loop, for catching up or comparing.

| Path | What it is |
|---|---|
| `loop.cs` | The loop after Labs 1-7 plus menu A (worktree) and menu C (OpenTelemetry). Replaces `starter/loop.cs`. Run it with the task file: `dotnet run loop.cs .workitems/T01/task.md`. |
| `app/.github/skills/plan/SKILL.md` | Lab 2. Copy into `starter/app/.github/skills/plan/`. |
| `app/.github/skills/implement/SKILL.md` | Lab 2, extended in Lab 4 (feedback file). Copy into `starter/app/.github/skills/implement/`. |
| `app/.github/skills/review/SKILL.md` | Lab 5, a longer version of the review skill on the page. Copy into `starter/app/.github/skills/review/`. |
| `app/.github/skills/improve/SKILL.md` | Lab 7. Not called by the loop: you run `/improve <work item folder>` yourself, in an interactive Copilot session in `app/`. Copy into `starter/app/.github/skills/improve/`. |
| `app/quality-gate.ps1` | Lab 3, rewritten in Lab 4 (output as instructions). Copy into `starter/app/`. |
| `app/.github/hooks/guard.json`, `guard.ps1` | Menu B: the guardrail as a repository hook. Copy into `starter/app/.github/hooks/`. |

Commit the copied files in `app/` before you run the loop: with menu A the loop works in a worktree that branches
from `main`, so it only sees what is committed.

Differences with the code on the workshop page, all deliberate (the page's code, lab by lab, is in `../snapshots/`):

- Menu C is applied: each step also writes its OpenTelemetry spans to `<step>.otel.jsonl`, next to its
  `<step>.events.jsonl` (Lab 6). Each step has its own Copilot runtime for that (`NewClient`).
- The plan step is skipped when `plan.md` exists, so a rerun continues in the same worktree (menu A).
- The program returns exit code 0 when the work item is committed and 1 when it stopped.
