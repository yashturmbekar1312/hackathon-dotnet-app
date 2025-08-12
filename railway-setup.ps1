# PowerShell script to configure Railway environment variables
# Run this script to set up your Railway deployment properly

Write-Host "Railway Deployment Setup for Personal Finance API" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Cyan

# Check if Railway CLI is installed
try {
    railway version | Out-Null
    Write-Host "✓ Railway CLI detected" -ForegroundColor Green
} catch {
    Write-Host "✗ Railway CLI not found. Please install it first:" -ForegroundColor Red
    Write-Host "  npm install -g @railway/cli" -ForegroundColor Yellow
    exit 1
}

Write-Host "`nSetting environment variables..." -ForegroundColor Yellow

# Set essential environment variables
Write-Host "Setting ASPNETCORE_ENVIRONMENT..." -ForegroundColor White
railway variables set ASPNETCORE_ENVIRONMENT=Production

Write-Host "Setting ASPNETCORE_URLS..." -ForegroundColor White
railway variables set ASPNETCORE_URLS="http://0.0.0.0:`$PORT"

# Prompt for JWT Secret Key
$jwtSecret = Read-Host "Enter JWT Secret Key (min 32 characters) [Press Enter for default]"
if ([string]::IsNullOrWhiteSpace($jwtSecret)) {
    $jwtSecret = "ProductionSecretKeyThatIsAtLeast32CharactersLongForProduction2025"
}
Write-Host "Setting JWT_SECRET_KEY..." -ForegroundColor White
railway variables set JWT_SECRET_KEY="$jwtSecret"

# Prompt for Brevo API Key
$brevoKey = Read-Host "Enter Brevo API Key (optional) [Press Enter to skip]"
if (![string]::IsNullOrWhiteSpace($brevoKey)) {
    Write-Host "Setting BREVO_API_KEY..." -ForegroundColor White
    railway variables set BREVO_API_KEY="$brevoKey"
}

Write-Host "`n" + "=" * 50 -ForegroundColor Cyan
Write-Host "Environment variables set successfully!" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Make sure you have a PostgreSQL database service in Railway" -ForegroundColor White
Write-Host "2. Railway will automatically set DATABASE_URL for your PostgreSQL service" -ForegroundColor White
Write-Host "3. Deploy your application: railway up" -ForegroundColor White
Write-Host "4. Check logs: railway logs" -ForegroundColor White

Write-Host "`nTroubleshooting:" -ForegroundColor Yellow
Write-Host "- If database connection fails, verify DATABASE_URL is set: railway variables" -ForegroundColor White
Write-Host "- Check database service status in Railway dashboard" -ForegroundColor White
Write-Host "- Review application logs: railway logs --follow" -ForegroundColor White

Write-Host "`nCurrent variables:" -ForegroundColor Cyan
railway variables
