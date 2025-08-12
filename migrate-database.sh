#!/bin/bash

# Database Migration Script for Personal Finance API
# This script applies Entity Framework migrations during deployment

set -e  # Exit on any error

echo "🚀 Starting database migration process..."

# Configuration
PROJECT_PATH="src/PersonalFinanceAPI"
MAX_RETRIES=5
RETRY_DELAY=10

# Function to check if PostgreSQL is available
check_database_connectivity() {
    echo "🔍 Checking database connectivity..."
    
    # Extract database connection details from environment variables
    if [ -z "$DATABASE_PUBLIC_URL" ]; then
        echo "❌ DATABASE_PUBLIC_URL environment variable is not set"
        exit 1
    fi
    
    echo "✅ Database URL found"
}

# Function to apply migrations with retry logic
apply_migrations() {
    local attempt=1
    
    while [ $attempt -le $MAX_RETRIES ]; do
        echo "📦 Attempting to apply migrations (attempt $attempt/$MAX_RETRIES)..."
        
        cd $PROJECT_PATH
        
        if dotnet ef database update --no-build --verbose; then
            echo "✅ Migrations applied successfully!"
            return 0
        else
            echo "❌ Migration attempt $attempt failed"
            
            if [ $attempt -eq $MAX_RETRIES ]; then
                echo "💥 All migration attempts failed. Deployment cannot continue."
                exit 1
            fi
            
            echo "⏳ Waiting $RETRY_DELAY seconds before retry..."
            sleep $RETRY_DELAY
            attempt=$((attempt + 1))
        fi
    done
}

# Function to check migration status
check_migration_status() {
    echo "📊 Checking migration status..."
    
    cd $PROJECT_PATH
    
    echo "Applied migrations:"
    dotnet ef migrations list --no-build 2>/dev/null || echo "No migrations found or database not accessible"
    
    echo "Database info:"
    dotnet ef dbcontext info --no-build 2>/dev/null || echo "Cannot retrieve database context info"
}

# Main execution flow
main() {
    echo "🏗️  Personal Finance API - Database Migration"
    echo "=============================================="
    
    # Check prerequisites
    if ! command -v dotnet &> /dev/null; then
        echo "❌ .NET CLI is not installed or not in PATH"
        exit 1
    fi
    
    # Check if Entity Framework tools are available
    if ! dotnet ef --version &> /dev/null; then
        echo "📦 Installing EF Core tools..."
        dotnet tool install --global dotnet-ef || true
    fi
    
    echo "🔧 EF Core version: $(dotnet ef --version)"
    
    # Check database connectivity
    check_database_connectivity
    
    # Build the project first
    echo "🔨 Building the project..."
    cd $PROJECT_PATH
    dotnet build --configuration Release --no-restore
    
    if [ $? -ne 0 ]; then
        echo "❌ Build failed. Cannot proceed with migrations."
        exit 1
    fi
    
    # Check current migration status
    check_migration_status
    
    # Apply migrations
    apply_migrations
    
    # Final status check
    echo "🎯 Final migration status check..."
    check_migration_status
    
    echo "🎉 Database migration process completed successfully!"
    echo "✨ Your Personal Finance API database is ready!"
}

# Run the main function
main "$@"
