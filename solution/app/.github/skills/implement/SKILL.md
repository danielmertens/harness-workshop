---
name: implement
description: Implements a work item from its plan, or fixes the feedback on an earlier attempt. Use when invoked as /implement <work item folder> [feedback file].
argument-hint: <work item folder> [feedback file]
---

# Implement a work item

Arguments:

1. A work item folder, for example `.workitems/T01`, with `plan.md`.
2. Optional: a feedback file about the previous attempt. Its changes are still in the working tree.
   - `gate-N.md` comes from the quality gate. Follow its instructions.
   - `review-N.json` comes from the code review. Fix every finding with severity `high` or `medium`. Ignore `low`.

## Steps

1. If there is a feedback file, read it first. Fixing it is your job now: do not start over.
2. Read `plan.md` in that folder.
3. Implement it: change the code and add the unit tests from the plan. Follow the style of the existing code.

## Rules

- Do not change `task.md`, anything under `.github/`, or `quality-gate.ps1`.
- Never delete, skip or weaken a test to make it pass.
- Do not commit. The harness does that.

Reply with a short summary: which files you changed and which tests you added.
