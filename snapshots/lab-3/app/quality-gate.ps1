# Quality gate of this repository. Exit code 0 = pass.

dotnet build
if ($LASTEXITCODE -ne 0) { exit 1 }

dotnet test --no-build
exit $LASTEXITCODE
