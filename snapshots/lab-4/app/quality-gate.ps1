# Quality gate of this repository. The development loop runs it after every implement step.
#   Exit code 0 = pass, anything else = fail.
#   Everything printed here goes to the agent that has to fix it, so write instructions, not a log.

$root = (Get-Location).Path + [IO.Path]::DirectorySeparatorChar
function Relative([string]$text) { $text.Replace($root, '') }

# 1. Build. Warnings count as errors in this repository (Directory.Build.props).
$build = dotnet build --nologo -v quiet -clp:NoSummary 2>&1 | ForEach-Object { "$_" }
if ($LASTEXITCODE -ne 0) {
    $errors = $build | Where-Object { $_ -match ': (error|warning) [A-Z]+\d+' } |
        ForEach-Object { Relative ($_ -replace '\s*\[[^\]]+\]\s*$', '') } | Sort-Object -Unique
    '# Quality gate: FAILED at build'
    ''
    'The code does not compile. Fix every error below.'
    ''
    if ($errors) { $errors | ForEach-Object { "- $_" } } else { $build | Select-Object -Last 30 }
    ''
    '## How to fix'
    ''
    '- Fix the cause in the code. Do not suppress warnings and do not delete code or tests to make an error go away.'
    '- Check your fix with `dotnet build`.'
    exit 1
}

# 2. Tests. The TRX report gives us each failing test with its message and location.
Remove-Item TestResults -Recurse -Force -ErrorAction SilentlyContinue
$test = dotnet test --no-build --nologo --logger 'trx;LogFileName=gate.trx' --results-directory TestResults 2>&1 | ForEach-Object { "$_" }
if ($LASTEXITCODE -eq 0) {
    '# Quality gate: PASSED'
    ''
    'The build is clean and all tests pass.'
    exit 0
}

$trx = Get-ChildItem TestResults -Recurse -Filter gate.trx | Select-Object -First 1
if (-not $trx) {
    '# Quality gate: FAILED at tests'
    ''
    'The test run did not finish. Its last output:'
    ''
    $test | Select-Object -Last 40
    exit 1
}

[xml]$report = Get-Content $trx.FullName -Raw
$failed = @($report.TestRun.Results.UnitTestResult | Where-Object outcome -eq 'Failed')
$total = $report.TestRun.ResultSummary.Counters.total

'# Quality gate: FAILED at tests'
''
"$($failed.Count) of $total tests fail."
foreach ($t in $failed) {
    $message = ($t.Output.ErrorInfo.Message -split "`r?`n" | ForEach-Object { "    $_" }) -join "`n"
    $frame = $t.Output.ErrorInfo.StackTrace -split "`r?`n" | Where-Object { $_ -match ' in (.+):line (\d+)' } | Select-Object -First 1
    $location = if ($frame -match ' in (.+):line (\d+)') { "$(Relative $Matches[1]):$($Matches[2])" } else { 'unknown' }
    ''
    "## $($t.testName)"
    ''
    "- Where: ``$location``"
    '- What failed:'
    ''
    $message
}
''
'## How to fix'
''
'- For each failing test, decide whether the code or the test is wrong.'
'- Fix the code when it does not do what the task asks. Change a test only when it contradicts the task.'
'- Never delete, skip or weaken a test to make it pass.'
exit 1
