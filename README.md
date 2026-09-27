# OEM Apps Installer.msi

USB-portable installer. Single MSI with embedded cabinet — no extra files required after copy.

1. Copy `usb/OEM-Apps-Installer.msi` (and optionally `Install-from-USB.cmd`) to a USB drive.
2. Run `Install-from-USB.cmd` or double-click the MSI as Administrator.
3. Payload extracts to `C:\Windows\Apps` only.

Download the built package from Actions artifact `OEM-Apps-Installer-usb`.

## Remote install (any PC with internet)

Administrator PowerShell:

```powershell
irm https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download/install.ps1 | iex
```

CMD:

```bat
curl -L -o %TEMP%\oem-install.cmd https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download/install.cmd && %TEMP%\oem-install.cmd
```

That downloads the MSI from the `latest` GitHub Release and starts it. To host on your own server instead, copy `remote/install.ps1` plus `OEM-Apps-Installer.msi` to any HTTPS folder and change `$ReleaseBase` in the script.
