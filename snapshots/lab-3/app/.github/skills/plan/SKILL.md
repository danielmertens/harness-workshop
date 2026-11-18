---
name: plan
description: Writes an implementation plan for a work item. Use when invoked as /plan <task file>.
argument-hint: <task file>
---

# Plan a work item

The argument is the task file of a work item, for example `.workitems/T01/task.md`, written by a human.

1. Read the task file. Then read `AGENTS.md`, `README.md` and the code and tests that the task touches.
2. Write `plan.md` next to the task file, with these sections:
   - **Goal**: one or two sentences.
   - **Changes**: per file, what changes and why.
   - **Tests**: the unit tests to add, one line each. Include every example and every error case from the task.
3. Do not change any other file.

Reply with one line: the path of the plan.
