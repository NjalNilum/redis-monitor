@echo off
REM Redis Monitor - Start Script
REM Double-click this file to start the Redis Monitor application

echo ===================================
echo  Redis Monitor - Starting...
echo ===================================
echo.

REM Start the application
dotnet run --project RedisMonitor.csproj

echo.
echo Application stopped.
pause
