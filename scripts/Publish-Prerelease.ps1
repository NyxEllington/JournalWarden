param(
    [string]$Configuration = "Release",
    [string]$RuntimeIdentifier = "win-x64",
    [string]$OutputRoot = $(Join-Path $PSScriptRoot "..\artifacts\prerelease")
)

$projectPath = Join-Path $PSScriptRoot "..\JournalWarden.csproj"
$artifactName = "JournalWarden-$RuntimeIdentifier"
$publishDir = Join-Path $OutputRoot $artifactName
$zipPath = Join-Path $OutputRoot "$artifactName.zip"

New-Item -ItemType Directory -Force -Path $OutputRoot | Out-Null

if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
}

if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

dotnet publish $projectPath -c $Configuration -r $RuntimeIdentifier --self-contained false -p:PublishSingleFile=false -p:DebugType=none -p:DebugSymbols=false -o $publishDir

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed."
    exit $LASTEXITCODE
}

Compress-Archive -Path (Join-Path $publishDir "*") -DestinationPath $zipPath -Force

Write-Host "Prerelease package created at $zipPath"
Write-Host "Extract the ZIP and run JournalWarden.exe"
