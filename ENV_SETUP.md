# Environment Variables Setup

This document explains how to set up environment variables for the Personal Finance API.

## Environment Files Created

- `.env` - Main environment file (for development)
- `.env.local` - Local development overrides
- `.env.production` - Production environment variables
- `.env.staging` - Staging environment variables
- `.env.example` - Template file (safe to commit)

## Required Environment Variables

### Email Service

```
BREVO_API_KEY=your_brevo_api_key_here
```

### JWT Configuration

```
JWT_SECRET_KEY=your_32_character_or_longer_secret_key
```

### Database (Optional - for Railway deployment)

```
DATABASE_PUBLIC_URL=postgresql://user:password@host:port/database
```

### ASP.NET Core Environment

```
ASPNETCORE_ENVIRONMENT=Development|Production|Staging
ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
```

## How to Use

### Option 1: PowerShell Script (Recommended)

```powershell
# For development
.\set-env-dev.ps1

# For production
.\set-env-prod.ps1
```

### Option 2: Batch File (Windows CMD)

```cmd
# For development
set-env-dev.bat
```

### Option 3: Manual Environment Variables

```powershell
# PowerShell
$env:BREVO_API_KEY="your_api_key"
$env:JWT_SECRET_KEY="your_jwt_secret"
```

```cmd
# Command Prompt
set BREVO_API_KEY=your_api_key
set JWT_SECRET_KEY=your_jwt_secret
```

### Option 4: Use .env file

The application will automatically load from `.env` file in the root directory.

## Security Notes

- ✅ `.env*` files are in `.gitignore` - they won't be committed
- ✅ Only `.env.example` should be committed to repository
- ✅ Use different API keys for different environments
- ✅ Keep production secrets secure and never share them

## Railway Deployment

For Railway deployment, set these environment variables in the Railway dashboard:

- `BREVO_API_KEY`
- `JWT_SECRET_KEY` (use a strong production key)
- `DATABASE_PUBLIC_URL` (will be auto-set by Railway for PostgreSQL)

## Verification

After setting environment variables, you can verify they're loaded by checking the application logs. The email service will use the API key from the environment variable instead of the empty value in `appsettings.json`.
