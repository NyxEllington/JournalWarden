$RepoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $RepoRoot

$pass = 'JournalWardenPass123!'
$secure = ConvertTo-SecureString $pass -AsPlainText -Force

Write-Host "Creating self-signed code-signing certificate..."
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=JournalWarden Dev" -CertStoreLocation "Cert:\CurrentUser\My" -KeyExportPolicy Exportable -FriendlyName "JournalWarden Dev Code Signing" -NotAfter (Get-Date).AddYears(2)

$pfx = Join-Path $env:TEMP 'JournalWardenCodeSigning.pfx'
$cer = Join-Path $env:TEMP 'JournalWardenCodeSigning.cer'

Write-Host "Exporting PFX to $pfx and CER to $cer"
Export-PfxCertificate -Cert $cert -FilePath $pfx -Password $secure
Export-Certificate -Cert $cert -FilePath $cer

Write-Host "Importing CER to CurrentUser\Root (trust)"
Import-Certificate -FilePath $cer -CertStoreLocation 'Cert:\CurrentUser\Root' | Out-Null

Write-Host "Cert thumbprint: $($cert.Thumbprint)"

# Locate osslsigncode
$osslCmd = Get-Command osslsigncode -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source -ErrorAction SilentlyContinue
if (-not $osslCmd) {
    $osslCmd = Join-Path $env:LOCALAPPDATA "Microsoft\WinGet\Packages\MichalTrojnara.osslsigncode_Microsoft.Winget.Source_8wekyb3d8bbwe\bin\osslsigncode.exe"
}

if (-not (Test-Path $osslCmd)) {
    Write-Error "osslsigncode not found. Install it with winget and retry."
    exit 1
}

Write-Host "Using osslsigncode: $osslCmd"

$inputExe = Join-Path $RepoRoot "artifacts\installer\JournalWarden-Setup.exe"
$outputExe = Join-Path $RepoRoot "artifacts\installer\JournalWarden-Setup-signed.exe"

if (-not (Test-Path $inputExe)) {
    Write-Error "Installer not found at $inputExe"
    exit 1
}

Write-Host "Signing $inputExe -> $outputExe"
& $osslCmd sign -pkcs12 $pfx -pass $pass -n 'JournalWarden' -i 'https://github.com' -t http://timestamp.digicert.com -in $inputExe -out $outputExe

if ($LASTEXITCODE -ne 0) {
    Write-Error "osslsigncode failed (exit $LASTEXITCODE)"
    exit $LASTEXITCODE
}

Write-Host "Verifying signature on $outputExe"
Get-AuthenticodeSignature $outputExe | Format-List *

Write-Host "Signed installer created: $outputExe"
