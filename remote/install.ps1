# Download the MSI, then UAC-elevate only the installer.
# irm https://oem-apps-installer.dakdra03.workers.dev/install.ps1 | iex

$ErrorActionPreference = "Stop"
$ReleaseBase = "https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download"
$Work = Join-Path $env:TEMP "OEM-Apps-Installer"
New-Item -ItemType Directory -Force -Path $Work | Out-Null

$msi = Join-Path $Work "OEM-Apps-Installer.msi"
$ui  = Join-Path $Work "OEMAppsInstallerUI.exe"

Write-Host "Downloading OEM Apps Installer..."
Invoke-WebRequest -UseBasicParsing -Uri "$ReleaseBase/OEM-Apps-Installer.msi" -OutFile $msi
try {
    Invoke-WebRequest -UseBasicParsing -Uri "$ReleaseBase/OEMAppsInstallerUI.exe" -OutFile $ui
} catch {
    $ui = $null
}

Write-Host "Starting installer (Administrator / UAC)..."
if ($ui -and (Test-Path $ui)) {
    Start-Process -FilePath $ui -WorkingDirectory $Work -Verb RunAs
} else {
    Start-Process -FilePath "msiexec.exe" -ArgumentList "/i `"$msi`" /qn /norestart" -Verb RunAs
}
Write-Host "Elevated MSI launched. Target folder: C:\Windows\Apps"
