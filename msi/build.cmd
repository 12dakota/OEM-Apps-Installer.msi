@echo off
setlocal
cd /d "%~dp0"

if not exist "payload\apps.zip" (
  echo Place apps.zip in msi\payload\apps.zip
  exit /b 1
)

dotnet publish ExtractToWindowsApps.csproj -c Release -o out-ca
if errorlevel 1 exit /b 1

dotnet tool restore 2>nul
where wix >nul 2>&1
if errorlevel 1 (
  dotnet tool install --global wix --version 4.0.5
)

wix build Package.wxs -o out\OemApps.msi -bindpath payload=payload -bindpath ca=out-ca
if errorlevel 1 exit /b 1

echo Built out\OemApps.msi
echo Install target: C:\Windows\Apps
echo MSI extracts payload\apps.zip into that directory.
