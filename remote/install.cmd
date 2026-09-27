@echo off
powershell -NoProfile -ExecutionPolicy Bypass -Command "irm https://github.com/12dakota/OEM-Apps-Installer.msi/releases/latest/download/install.ps1 | iex"
