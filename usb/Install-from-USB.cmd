@echo off
setlocal
cd /d "%~dp0"
net session >nul 2>&1
if errorlevel 1 (
  powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
  exit /b
)
if exist "%~dp0OEMAppsInstallerUI.exe" (
  start "" "%~dp0OEMAppsInstallerUI.exe"
  exit /b 0
)
msiexec /i "%~dp0OEM-Apps-Installer.msi" /qn /norestart
