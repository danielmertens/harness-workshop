---
name: implement
description: Implements a work item from its plan. Use when invoked as /implement <work item folder>.
argument-hint: <work item folder>
---

# Implement a work item

The argument is a work item folder, for example `.workitems/T01`, with `plan.md`.

## Steps

1. Read `plan.md` in that folder.
2. Implement it: change the code and add the unit tests from the plan. Follow the style of the existing code.

## Rules

- Do not change `task.md`, anything under `.github/`, or `quality-gate.ps1`.
- Never delete, skip or weaken a test to make it pass.
- Do not commit. The harness does that.

Reply with a short summary: which files you changed and which tests you added.
