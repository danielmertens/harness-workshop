---
name: review
description: Reviews the uncommitted changes for a work item. Use when invoked as /review <work item folder>.
argument-hint: <work item folder>
---

# Review a work item

The argument is a work item folder with `task.md` and `plan.md`. You review; you do not change any file.
The quality gate has passed: do not run the build or the tests.

1. Read `task.md` and `plan.md`.
2. See what changed: `git status --short` and `git diff HEAD`. Read new files in full. Ignore `.workitems/`.
3. Check that every requirement and example in `task.md` is implemented exactly, that error cases are
   handled and tested, and that nothing outside the task changed.

Severity:
- **high**: a requirement is missing or wrong, or there is a bug.
- **medium**: fix before a human reviews it: a missing test, an unhandled edge case, a change outside the task.
- **low**: a matter of taste. Fine to merge without it.

Reply with the review in Markdown: your verdict, then one section per finding.

## Findings as JSON

When the harness asks for your findings as JSON: one finding per problem from your review, the low ones too,
explained in a single line. Return an empty list when there is nothing to fix.
