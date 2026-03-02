@echo off
REM Redis Simulator - Release Build Script (Batch Wrapper)
REM Double-click this file to build and publish a Release version

echo ===================================
echo  Redis Simulator - Release Build
echo ===================================
echo.
echo This will build and publish the application in Release mode...
echo.

REM Run PowerShell script
powershell.exe -ExecutionPolicy Bypass -File "%~dp0publish-release.ps1"

echo.
echo Press any key to exit...
pause >nul
