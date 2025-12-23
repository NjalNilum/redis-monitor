# Redis Monitor - Complete Deployment Script
# This script creates a complete deployment package

param(
    [string]$DeploymentDir = ".\deployment",
    [string]$Configuration = "Release"
)

Write-Host "====================================" -ForegroundColor Cyan
Write-Host " Redis Monitor - Deployment" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Create deployment directory structure
$publishDir = Join-Path $DeploymentDir "RedisMonitor"
$configDir = Join-Path $publishDir "config"

Write-Host "Creating deployment structure..." -ForegroundColor Yellow
if (Test-Path $DeploymentDir) {
    Remove-Item -Path $DeploymentDir -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDir -Force | Out-Null
New-Item -ItemType Directory -Path $configDir -Force | Out-Null

# Build and publish
Write-Host "Building and publishing..." -ForegroundColor Yellow
dotnet publish --configuration $Configuration --output $publishDir --no-self-contained

if ($LASTEXITCODE -ne 0) {
    Write-Host "Error: Publish failed" -ForegroundColor Red
    exit 1
}

# Copy configuration templates
Write-Host ""
Write-Host "Setting up configuration..." -ForegroundColor Yellow
Copy-Item "appsettings.json" -Destination $configDir
if (Test-Path "appsettings.Development.json") {
    Copy-Item "appsettings.Development.json" -Destination $configDir
}

# Copy run script
Write-Host "Adding startup script..." -ForegroundColor Yellow
Copy-Item "run-release.bat" -Destination $publishDir

# Create deployment readme
$readmeContent = @"
# Redis Monitor - Deployment Package

## Requirements
- .NET 10.0 Runtime (or SDK)
- Redis Server (local or remote)

## Installation

1. Extract this folder to your desired location
2. Configure Redis connection:
   - Edit ``appsettings.json`` in the application directory
   - Set your Redis connection string and other settings

## Running the Application

### Option 1: Double-Click
- Double-click ``run-release.bat``

### Option 2: Command Line
- Open terminal in this directory
- Run: ``RedisMonitor.exe``

The application will start and open in your default browser.
Default URL: http://localhost:5000

## Configuration

Configuration files in the ``config`` folder contain templates.
Edit ``appsettings.json`` to customize:
- Redis connection string
- Port settings
- Monitoring options

## Stopping the Application

- Close the terminal window, or
- Press Ctrl+C in the terminal

## Support

For issues or questions, refer to the project documentation.
"@

Set-Content -Path (Join-Path $DeploymentDir "README.txt") -Value $readmeContent

# Create version info
$versionInfo = @"
Redis Monitor
Build Date: $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")
Configuration: $Configuration
.NET Version: 10.0
"@

Set-Content -Path (Join-Path $publishDir "VERSION.txt") -Value $versionInfo

Write-Host ""
Write-Host "====================================" -ForegroundColor Green
Write-Host " Deployment Complete!" -ForegroundColor Green
Write-Host "====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Deployment package created at:" -ForegroundColor Cyan
Write-Host "  $DeploymentDir" -ForegroundColor White
Write-Host ""
Write-Host "Contents:" -ForegroundColor Yellow
Write-Host "  - Application files" -ForegroundColor White
Write-Host "  - Configuration templates" -ForegroundColor White
Write-Host "  - Startup script (run-release.bat)" -ForegroundColor White
Write-Host "  - Documentation (README.txt)" -ForegroundColor White
Write-Host ""
Write-Host "To distribute:" -ForegroundColor Yellow
Write-Host "  - Zip the '$DeploymentDir' folder" -ForegroundColor White
Write-Host "  - Share with users" -ForegroundColor White
Write-Host ""
