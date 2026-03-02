# Redis Simulator - Release Build & Publish Script
# This script builds and publishes the application in Release mode

param(
    [string]$OutputDir = ".\publish",
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

Write-Host "======================================" -ForegroundColor Cyan
Write-Host " Redis Simulator - Release Build" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

$Project = Join-Path $PSScriptRoot "RedisSimulator.csproj"

# Clean previous publish
Write-Host "Cleaning previous publish output..." -ForegroundColor Yellow
$ResolvedOutputDir = Join-Path $PSScriptRoot $OutputDir
if (Test-Path $ResolvedOutputDir) {
    Remove-Item -Path $ResolvedOutputDir -Recurse -Force
    Write-Host "Cleaned: $ResolvedOutputDir" -ForegroundColor Green
}

# Restore dependencies
Write-Host ""
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore $Project --runtime $Runtime

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Failed to restore dependencies" -ForegroundColor Red
    exit 1
}

# Build Release version
Write-Host ""
Write-Host "Building $Configuration version..." -ForegroundColor Yellow
dotnet build $Project --configuration $Configuration --runtime $Runtime --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Build failed" -ForegroundColor Red
    exit 1
}

# Publish as framework-dependent executable (.exe apphost)
Write-Host ""
Write-Host "Publishing to $ResolvedOutputDir for runtime $Runtime..." -ForegroundColor Yellow
dotnet publish $Project --configuration $Configuration --runtime $Runtime --self-contained false -p:UseAppHost=true --output $ResolvedOutputDir --no-build --no-restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Publish failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host " Build Complete!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "Published to: $ResolvedOutputDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "To run the application:" -ForegroundColor Yellow
Write-Host "  cd $ResolvedOutputDir" -ForegroundColor White
Write-Host "  .\RedisSimulator.exe" -ForegroundColor White
Write-Host ""
