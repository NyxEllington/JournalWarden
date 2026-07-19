# Journal Warden Launch Script
Write-Host "================================================" -ForegroundColor Cyan
Write-Host "   Journal Warden - Elite Dangerous Monitor" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Building and launching..." -ForegroundColor Yellow
Write-Host ""

dotnet run

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Error occurred." -ForegroundColor Red
    Read-Host "Press Enter to exit"
}
