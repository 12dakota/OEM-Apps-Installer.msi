@echo off
setlocal
cd /d "%~dp0"
echo OEM Apps Installer
echo Installs to C:\Windows\Apps
echo This package is self-contained. Keep the USB plugged in until msiexec finishes.
echo.
net session >nul 2>&1
if errorlevel 1 (
  echo Requesting Administrator...
  powershell -NoProfile -Command "Start-Process -FilePath '%~f0' -Verb RunAs"
  exit /b
)
msiexec /i "%~dp0OEM-Apps-Installer.msi" /qn /norestart /l*v "%TEMP%\OEM-Apps-Installer.log"
set ERR=%ERRORLEVEL%
echo msiexec exit %ERR%
echo Log: %TEMP%\OEM-Apps-Installer.log
if exist "C:\Windows\Apps\.oem-install-complete" echo Install complete: C:\Windows\Apps
exit /b %ERR%
