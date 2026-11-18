---
name: improve
description: Studies how the development loop handled one or more work items and proposes changes that make the loop stronger. Use when invoked as /improve <work item folder> [more work item folders].
argument-hint: <work item folder> [more work item folders]
---

# Improve the development loop

The arguments are work item folders that have been through the development loop. Each holds `task.md`, `plan.md`,
a `gate-N.md` per attempt, `review-N.md` and `review-N.json` per review, and an event log per step:
`<step>.events.jsonl`, one JSON event per line. The logs are large: search them, do not read them in full.

The loop is configured by files in this repository: the skills in `.github/skills/`, `quality-gate.ps1`,
and the hooks in `.github/hooks/`, if there are any.

1. Per work item, reconstruct what happened: the attempts, why each one failed, which tool calls failed or were
   denied, and what the agent spent most of its tool calls on. With several work items, look for what repeats.
2. Look for what cost time or trust, and find its cause in that configuration. For example: a gate failure or a
   review finding that a clearer skill would have prevented, a review finding that the gate could check,
   a rule that the agent broke or tried to break, the same command run again and again, feedback that was misread.
3. Do not change any file. The developer decides which proposals to apply.

Reply in Markdown with at most five proposals, most valuable first. For each: the file to change, the change
itself (the exact text to add or replace), and the evidence (a file and line, or an event) that shows why.
If the runs went smoothly, say so: no proposals is a fine answer.
