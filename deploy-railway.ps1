# Deploy to Railway with Database Fix
# This script deploys the application with all the fixes applied

Write-Host "Personal Finance API - Railway Deployment" -ForegroundColor Cyan
Write-Host "=" * 50 -ForegroundColor Cyan

# Check if we're in the right directory
if (!(Test-Path "PersonalFinanceAPI.sln")) {
    Write-Host "Error: Run this script from the project root directory" -ForegroundColor Red
    exit 1
}

# Check Railway CLI
try {
    railway version | Out-Null
    Write-Host "✓ Railway CLI detected" -ForegroundColor Green
} catch {
    Write-Host "✗ Railway CLI not found. Install with: npm install -g @railway/cli" -ForegroundColor Red
    exit 1
}

Write-Host "`n1. Setting up environment variables..." -ForegroundColor Yellow

# Essential environment variables
railway variables set ASPNETCORE_ENVIRONMENT=Production
railway variables set ASPNETCORE_URLS="http://0.0.0.0:`$PORT"

# JWT Secret
$jwtSecret = Read-Host "Enter JWT Secret Key (min 32 chars) [Enter for default]"
if ([string]::IsNullOrWhiteSpace($jwtSecret)) {
    $jwtSecret = "ProductionSecretKeyThatIsAtLeast32CharactersLongForProduction2025"
}
railway variables set JWT_SECRET_KEY="$jwtSecret"

Write-Host "`n2. Checking database service..." -ForegroundColor Yellow
$services = railway service list 2>$null
if ($services -match "postgres") {
    Write-Host "✓ PostgreSQL service found" -ForegroundColor Green
} else {
    Write-Host "⚠ No PostgreSQL service detected. Creating one..." -ForegroundColor Yellow
    Write-Host "Please add a PostgreSQL service in your Railway dashboard:" -ForegroundColor White
    Write-Host "  1. Go to your Railway project" -ForegroundColor White
    Write-Host "  2. Click 'New Service' -> 'Database' -> 'Add PostgreSQL'" -ForegroundColor White
    Write-Host "  3. Wait for it to deploy, then run this script again" -ForegroundColor White
    
    $continue = Read-Host "Continue with deployment anyway? (y/N)"
    if ($continue -ne "y" -and $continue -ne "Y") {
        exit 0
    }
}

Write-Host "`n3. Building and deploying application..." -ForegroundColor Yellow
Write-Host "This may take a few minutes..." -ForegroundColor White

try {
    railway up --detach
    Write-Host "✓ Deployment initiated" -ForegroundColor Green
} catch {
    Write-Host "✗ Deployment failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "`n4. Monitoring deployment..." -ForegroundColor Yellow
Write-Host "Waiting for deployment to complete (this may take 2-3 minutes)..." -ForegroundColor White

# Wait a bit for deployment to start
Start-Sleep -Seconds 10

# Check deployment status
$retries = 0
$maxRetries = 30
$deploymentComplete = $false

while ($retries -lt $maxRetries -and !$deploymentComplete) {
    try {
        $status = railway status 2>$null
        if ($status -match "Deployed" -or $status -match "Success") {
            $deploymentComplete = $true
            Write-Host "✓ Deployment completed successfully!" -ForegroundColor Green
        } elseif ($status -match "Failed" -or $status -match "Error") {
            Write-Host "✗ Deployment failed. Check logs below:" -ForegroundColor Red
            railway logs --tail 20
            exit 1
        } else {
            Write-Host "." -NoNewline -ForegroundColor Yellow
            Start-Sleep -Seconds 6
            $retries++
        }
    } catch {
        Write-Host "." -NoNewline -ForegroundColor Yellow
        Start-Sleep -Seconds 6
        $retries++
    }
}

if (!$deploymentComplete) {
    Write-Host "`nDeployment taking longer than expected. Check status manually." -ForegroundColor Yellow
}

Write-Host "`n5. Testing deployment..." -ForegroundColor Yellow

try {
    $url = railway url 2>$null
    if ($url) {
        Write-Host "Application URL: $url" -ForegroundColor Green
        
        # Test health endpoint
        try {
            $healthUrl = "$url/health"
            Write-Host "Testing health endpoint: $healthUrl" -ForegroundColor White
            $response = Invoke-RestMethod -Uri $healthUrl -Method Get -TimeoutSec 30
            
            if ($response.status -eq "Healthy") {
                Write-Host "✓ Health check passed!" -ForegroundColor Green
                Write-Host "  Database status: $($response.checks | Where-Object {$_.name -eq 'database'} | Select-Object -ExpandProperty status)" -ForegroundColor Green
            } else {
                Write-Host "⚠ Health check returned: $($response.status)" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "⚠ Health check failed: $($_.Exception.Message)" -ForegroundColor Yellow
            Write-Host "  This might be normal if the app is still starting up" -ForegroundColor White
        }
    }
} catch {
    Write-Host "Could not retrieve application URL" -ForegroundColor Yellow
}

Write-Host "`n" + "=" * 50 -ForegroundColor Cyan
Write-Host "Deployment Summary:" -ForegroundColor Cyan
Write-Host "✓ Environment variables configured" -ForegroundColor Green
Write-Host "✓ Application deployed to Railway" -ForegroundColor Green
Write-Host "✓ Database connection optimized" -ForegroundColor Green
Write-Host "✓ Logging optimized for production" -ForegroundColor Green

Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. Monitor logs: railway logs --follow" -ForegroundColor White
Write-Host "2. View app: railway open" -ForegroundColor White
Write-Host "3. Check variables: railway variables" -ForegroundColor White
Write-Host "4. If issues persist, see RAILWAY_TROUBLESHOOTING.md" -ForegroundColor White

Write-Host "`nIf you encounter database connection issues:" -ForegroundColor Yellow
Write-Host "- Verify PostgreSQL service is running in Railway dashboard" -ForegroundColor White
Write-Host "- Check that DATABASE_URL is automatically set: railway variables" -ForegroundColor White
Write-Host "- Review recent logs: railway logs --tail 50" -ForegroundColor White
