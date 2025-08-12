# Database Migration Script for Personal Finance API (Windows PowerShell)
# This script applies Entity Framework migrations during deployment

param(
    [int]$MaxRetries = 5,
    [int]$RetryDelaySeconds = 10,
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Starting database migration process..." -ForegroundColor Green

# Configuration
$ProjectPath = "src\PersonalFinanceAPI"
$ScriptPath = $PSScriptRoot

function Test-DatabaseConnectivity {
    Write-Host "🔍 Checking database connectivity..." -ForegroundColor Yellow
    
    # Check if DATABASE_PUBLIC_URL environment variable is set
    $databaseUrl = $env:DATABASE_PUBLIC_URL
    if ([string]::IsNullOrEmpty($databaseUrl)) {
        Write-Host "❌ DATABASE_PUBLIC_URL environment variable is not set" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "✅ Database URL found" -ForegroundColor Green
}

function Invoke-MigrationsWithRetry {
    param([int]$MaxAttempts, [int]$DelaySeconds)
    
    $attempt = 1
    
    while ($attempt -le $MaxAttempts) {
        Write-Host "📦 Attempting to apply migrations (attempt $attempt/$MaxAttempts)..." -ForegroundColor Yellow
        
        Push-Location $ProjectPath
        
        try {
            $result = dotnet ef database update --no-build --verbose
            if ($LASTEXITCODE -eq 0) {
                Write-Host "✅ Migrations applied successfully!" -ForegroundColor Green
                Pop-Location
                return $true
            }
            else {
                throw "Migration command failed with exit code $LASTEXITCODE"
            }
        }
        catch {
            Write-Host "❌ Migration attempt $attempt failed: $($_.Exception.Message)" -ForegroundColor Red
            
            if ($attempt -eq $MaxAttempts) {
                Write-Host "💥 All migration attempts failed. Deployment cannot continue." -ForegroundColor Red
                Pop-Location
                exit 1
            }
            
            Write-Host "⏳ Waiting $DelaySeconds seconds before retry..." -ForegroundColor Yellow
            Start-Sleep -Seconds $DelaySeconds
            $attempt++
        }
        finally {
            Pop-Location
        }
    }
    
    return $false
}

function Get-MigrationStatus {
    Write-Host "📊 Checking migration status..." -ForegroundColor Yellow
    
    Push-Location $ProjectPath
    
    try {
        Write-Host "Applied migrations:" -ForegroundColor Cyan
        $migrations = dotnet ef migrations list --no-build 2>$null
        if ($LASTEXITCODE -eq 0) {
            $migrations
        } else {
            Write-Host "No migrations found or database not accessible" -ForegroundColor Gray
        }
        
        Write-Host "`nDatabase info:" -ForegroundColor Cyan
        $dbInfo = dotnet ef dbcontext info --no-build 2>$null
        if ($LASTEXITCODE -eq 0) {
            $dbInfo
        } else {
            Write-Host "Cannot retrieve database context info" -ForegroundColor Gray
        }
    }
    finally {
        Pop-Location
    }
}

function Main {
    Write-Host "🏗️  Personal Finance API - Database Migration" -ForegroundColor Magenta
    Write-Host "==============================================" -ForegroundColor Magenta
    
    # Check prerequisites
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
        Write-Host "❌ .NET CLI is not installed or not in PATH" -ForegroundColor Red
        exit 1
    }
    
    # Check if Entity Framework tools are available
    $efVersion = dotnet ef --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Host "📦 Installing EF Core tools..." -ForegroundColor Yellow
        dotnet tool install --global dotnet-ef
        $efVersion = dotnet ef --version 2>$null
    }
    
    Write-Host "🔧 EF Core version: $efVersion" -ForegroundColor Cyan
    
    # Check database connectivity
    Test-DatabaseConnectivity
    
    # Build the project first
    Write-Host "🔨 Building the project..." -ForegroundColor Yellow
    Push-Location $ProjectPath
    
    try {
        dotnet build --configuration $Configuration --no-restore
        if ($LASTEXITCODE -ne 0) {
            Write-Host "❌ Build failed. Cannot proceed with migrations." -ForegroundColor Red
            exit 1
        }
    }
    finally {
        Pop-Location
    }
    
    # Check current migration status
    Get-MigrationStatus
    
    # Apply migrations
    $success = Invoke-MigrationsWithRetry -MaxAttempts $MaxRetries -DelaySeconds $RetryDelaySeconds
    
    if (-not $success) {
        Write-Host "❌ Migration process failed" -ForegroundColor Red
        exit 1
    }
    
    # Final status check
    Write-Host "🎯 Final migration status check..." -ForegroundColor Yellow
    Get-MigrationStatus
    
    Write-Host "🎉 Database migration process completed successfully!" -ForegroundColor Green
    Write-Host "✨ Your Personal Finance API database is ready!" -ForegroundColor Green
}

# Run the main function
try {
    Main
}
catch {
    Write-Host "💥 An unexpected error occurred: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
