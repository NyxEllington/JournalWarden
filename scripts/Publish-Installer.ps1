param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64"
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..")
$publishScript = Join-Path $scriptDir "Publish-Prerelease.ps1"
$issScript = Join-Path $scriptDir "JournalWarden.iss"
$installerOutputDir = Join-Path $repoRoot "artifacts\installer"

& $publishScript -Configuration $Configuration -RuntimeIdentifier $RuntimeIdentifier

if ($LASTEXITCODE -ne 0) {
    Write-Error "Prerelease package creation failed."
    exit $LASTEXITCODE
}

$innoCompiler = Join-Path ${env:ProgramFiles(x86)} "Inno Setup 6\ISCC.exe"
if (-not (Test-Path $innoCompiler)) {
    $innoCompiler = Join-Path $env:ProgramFiles "Inno Setup 6\ISCC.exe"
}
if (-not (Test-Path $innoCompiler)) {
    $innoCompiler = "C:\Inno Setup 6\ISCC.exe"
}

if (-not (Test-Path $innoCompiler)) {
    Write-Error "Inno Setup compiler not found. Install it first or run the script after installing Inno Setup."
    exit 1
}

New-Item -ItemType Directory -Force -Path $installerOutputDir | Out-Null
& $innoCompiler $issScript

if ($LASTEXITCODE -ne 0) {
    Write-Error "Installer build failed."
    exit $LASTEXITCODE
}

Write-Host "Installer created in $installerOutputDir"
