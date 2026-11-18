# Guardrail of this repository. Copilot runs it before every tool call, in every session in this repository:
# the development loop and a developer working by hand alike.
# Input (stdin): one JSON object with toolName and toolArgs. Output: nothing to allow, or a deny with a reason for the agent.
$call = [Console]::In.ReadToEnd() | ConvertFrom-Json
$tool = if ($call.toolArgs -is [string]) { $call.toolArgs | ConvertFrom-Json } else { $call.toolArgs }

function Deny([string]$reason) {
    @{ permissionDecision = 'deny'; permissionDecisionReason = $reason } | ConvertTo-Json -Compress
    exit 0
}

# The rules are owned by the developers: the task, the skills, the hooks and the quality gate.
$path = "$($tool.path)" -replace '\\', '/'
if ($call.toolName -in 'create', 'edit' -and $path -match '/(task\.md|quality-gate\.ps1)$|/\.github/') {
    Deny "$($tool.path) is owned by the developers. Change the code, not the rules."
}

# The harness commits, after the gate and the review.
if ($call.toolName -in 'bash', 'powershell' -and "$($tool.command)" -match 'git\s+(commit|push)') {
    Deny 'The harness commits. You only change files.'
}
