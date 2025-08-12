# PowerShell script to set environment variables for development
# Run this script before starting the application: .\set-env-dev.ps1

Write-Host "Setting environment variables for development..." -ForegroundColor Green

# Email Service Configuration
$env:BREVO_API_KEY = "your_brevo_api_key_here"

# JWT Configuration
$env:JWT_SECRET_KEY = "DevelopmentSecretKeyThatIsAtLeast32CharactersLongForDevelopment2025"

# Development Environment
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5000;https://localhost:5001"

# Database (if using environment variable override)
# $env:DATABASE_PUBLIC_URL = "postgresql://postgres:root@localhost:5432/personal_finance_db"

Write-Host "Environment variables set successfully!" -ForegroundColor Green
Write-Host "You can now run: dotnet run" -ForegroundColor Yellow
