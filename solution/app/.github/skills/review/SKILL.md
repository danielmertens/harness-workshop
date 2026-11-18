---
name: review
description: Reviews the uncommitted changes for a work item against its task and plan. Use when invoked as /review <work item folder>.
argument-hint: <work item folder>
---

# Review a work item

The argument is a work item folder, for example `.workitems/T01`. It contains `task.md` (what a human asked for)
and usually `plan.md` (how it was planned).

You review. You do not change any file.

## What has already been checked

The quality gate has passed: the code builds and all tests are green. Do not run the build or the tests again.
Spend your time on what a build and a test run cannot see.

## Steps

1. Read `task.md` and `plan.md` in the work item folder.
2. See what changed: `git status --short` and `git diff HEAD`. New files show up as untracked (`??`): read them in full.
   Ignore everything under `.workitems/`.
3. Check, in this order:
   - **Requirements.** Go through `task.md` line by line. Is every requirement and every example implemented exactly
     as written (values, messages, exit codes, rounding)?
   - **Correctness.** Edge cases, error handling, input that the examples do not mention.
   - **Tests.** Do the new tests cover the requirements and the error cases? Would they fail if the feature broke?
   - **Scope.** Changes that the task did not ask for.
   - **Code.** Readability and consistency with the existing code.

## Severity

- **high**: a requirement from `task.md` is missing or wrong, or the change introduces a bug.
- **medium**: should be fixed before a human reviews it: a missing test for a requirement or an error case,
  an unhandled edge case, a change outside the scope of the task.
- **low**: a matter of taste or a small improvement. Fine to merge without it.

Only report real problems. "Looks good" is not a finding.

## Output

Reply with the review in Markdown:

- One line with your verdict.
- Then one section per finding: severity, file and line, what is wrong, and what the fix is.

## Findings as JSON

After your review, the harness asks you for your findings as JSON, in a format it defines. Then:

- Give one finding per problem from your review, the low ones too. Do not add new ones.
- Use the severities as defined above.
- Explain each finding in a single line: the file, what is wrong and what the fix is.
- Return an empty list when there is nothing to fix. Do not list things that are fine.
