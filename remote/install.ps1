# OEM Apps Installer — run elevated on the target PC
# irm https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download/install.ps1 | iex

$ErrorActionPreference = "Stop"
$ReleaseBase = "https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download"
$Work = Join-Path $env:TEMP "OEM-Apps-Installer"
New-Item -ItemType Directory -Force -Path $Work | Out-Null

function Assert-Admin {
    $id = [Security.Principal.WindowsIdentity]::GetCurrent()
    $p = New-Object Security.Principal.WindowsPrincipal($id)
    if (-not $p.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        $script = $MyInvocation.MyCommand.Path
        if ($script) {
            Start-Process powershell -Verb RunAs -ArgumentList "-NoProfile -ExecutionPolicy Bypass -File `"$script`""
        } else {
            $cmd = "irm $ReleaseBase/install.ps1 | iex"
            Start-Process powershell -Verb RunAs -ArgumentList "-NoProfile -ExecutionPolicy Bypass -Command $cmd"
        }
        exit 0
    }
}

Assert-Admin

$msi = Join-Path $Work "OEM-Apps-Installer.msi"
$ui  = Join-Path $Work "OEMAppsInstallerUI.exe"
Write-Host "Downloading OEM Apps Installer..."
Invoke-WebRequest -UseBasicParsing -Uri "$ReleaseBase/OEM-Apps-Installer.msi" -OutFile $msi
try {
    Invoke-WebRequest -UseBasicParsing -Uri "$ReleaseBase/OEMAppsInstallerUI.exe" -OutFile $ui
} catch {
    $ui = $null
}

if ($ui -and (Test-Path $ui)) {
    Copy-Item $msi (Join-Path $Work "OEM-Apps-Installer.msi") -Force
    Start-Process -FilePath $ui -WorkingDirectory $Work
} else {
    Start-Process -FilePath "msiexec.exe" -ArgumentList "/i `"$msi`" /qn /norestart" -Wait
}
Write-Host "Started. Target folder: C:\Windows\Apps"
