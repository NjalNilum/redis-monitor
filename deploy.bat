@echo off
REM Redis Monitor - Complete Deployment Script (Batch Wrapper)
REM Double-click this file to create a complete deployment package

echo ===================================
echo  Redis Monitor - Deployment
echo ===================================
echo.
echo This will create a complete deployment package...
echo.

REM Run PowerShell script
powershell.exe -ExecutionPolicy Bypass -File "%~dp0deploy.ps1"

echo.
echo Press any key to exit...
pause >nul
