#!/bin/bash

# Simple script to check migration status
echo "🔍 Checking Entity Framework migrations..."

cd src/PersonalFinanceAPI

echo "📋 Available migrations:"
dotnet ef migrations list

echo ""
echo "📊 Database context info:"
dotnet ef dbcontext info

echo ""
echo "✅ Migration check complete!"
