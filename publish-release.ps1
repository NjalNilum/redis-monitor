# Redis Monitor - Release Build & Publish Script
# This script builds and publishes the application in Release mode

param(
    [string]$OutputDir = ".\publish",
    [string]$Configuration = "Release"
)

Write-Host "====================================" -ForegroundColor Cyan
Write-Host " Redis Monitor - Release Build" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous build
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $OutputDir) {
    Remove-Item -Path $OutputDir -Recurse -Force
    Write-Host "Cleaned: $OutputDir" -ForegroundColor Green
}

# Restore dependencies
Write-Host ""
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to restore dependencies" -ForegroundColor Red
    exit 1
}

# Build Release version
Write-Host ""
Write-Host "Building $Configuration version..." -ForegroundColor Yellow
dotnet build --configuration $Configuration --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Build failed" -ForegroundColor Red
    exit 1
}

# Publish the application
Write-Host ""
Write-Host "Publishing to $OutputDir..." -ForegroundColor Yellow
dotnet publish --configuration $Configuration --output $OutputDir --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Publish failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "====================================" -ForegroundColor Green
Write-Host " Build Complete!" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Published to: $OutputDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "To run the application:" -ForegroundColor Yellow
Write-Host "  cd $OutputDir" -ForegroundColor White
Write-Host "  .\RedisMonitor.exe" -ForegroundColor White
Write-Host ""
