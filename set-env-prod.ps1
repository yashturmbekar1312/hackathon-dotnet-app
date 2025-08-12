# PowerShell script to set environment variables for production
# Run this script before deploying: .\set-env-prod.ps1

Write-Host "Setting environment variables for production..." -ForegroundColor Green

# Email Service Configuration
$env:BREVO_API_KEY = "your_brevo_api_key_here"

# JWT Configuration (Use a strong production key)
$env:JWT_SECRET_KEY = "ProductionSecretKeyThatIsAtLeast32CharactersLongForProduction2025"

# Production Environment
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://0.0.0.0:8080"

Write-Host "Production environment variables set successfully!" -ForegroundColor Green
Write-Host "Make sure to update DATABASE_PUBLIC_URL for your production database" -ForegroundColor Yellow
