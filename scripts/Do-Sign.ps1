$ErrorActionPreference = 'Stop'

$pass = 'JournalWardenPass123!'
$secure = ConvertTo-SecureString $pass -AsPlainText -Force

Write-Host "Creating self-signed code-signing certificate..."
$cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=JournalWarden Dev" -CertStoreLocation "Cert:\CurrentUser\My" -KeyExportPolicy Exportable -FriendlyName "JournalWarden Dev Code Signing" -NotAfter (Get-Date).AddYears(2)

$pfx = Join-Path $env:TEMP "JournalWardenCodeSigning.pfx"
$cer = Join-Path $env:TEMP "JournalWardenCodeSigning.cer"

Write-Host "Exporting PFX to $pfx and CER to $cer..."
Export-PfxCertificate -Cert $cert -FilePath $pfx -Password $secure
Export-Certificate -Cert $cert -FilePath $cer

Write-Host "Importing CER into CurrentUser\Root (trusting cert)..."
Import-Certificate -FilePath $cer -CertStoreLocation "Cert:\CurrentUser\Root" | Out-Null

Write-Host "Cert thumbprint: $($cert.Thumbprint)"
Write-Host "PFX path: $pfx"

$osslCandidate = Join-Path $env:LOCALAPPDATA 'Microsoft\WinGet\Packages\MichalTrojnara.osslsigncode_Microsoft.Winget.Source_8wekyb3d8bbwe\bin\osslsigncode.exe'
if (Test-Path $osslCandidate) { $ossl = $osslCandidate } else { $ossl = 'osslsigncode' }

$inPath = (Resolve-Path '.\artifacts\installer\JournalWarden-Setup.exe').Path
$outPath = (Join-Path (Split-Path $inPath) 'JournalWarden-Setup-signed.exe')

Write-Host "Signing $inPath -> $outPath with osslsigncode ($ossl) ..."
& $ossl sign -pkcs12 $pfx -pass $pass -n 'JournalWarden' -i 'https://github.com' -t http://timestamp.digicert.com -in $inPath -out $outPath

Write-Host "Verifying signature on $outPath"
Get-AuthenticodeSignature $outPath | Format-List *

Write-Host "Signed installer path: $outPath"
