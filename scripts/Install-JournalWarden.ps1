$repoRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$installerPath = Join-Path $repoRoot "artifacts\installer\JournalWarden-Setup.exe"

if (-not (Test-Path $installerPath)) {
    Write-Error "Installer not found at $installerPath"
    exit 1
}

Write-Host "Unblocking installer..."
Unblock-File -Path $installerPath -ErrorAction SilentlyContinue

Write-Host "Launching installer..."
Start-Process -FilePath $installerPath -Wait
