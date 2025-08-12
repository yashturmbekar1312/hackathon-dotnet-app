# Railway Deployment Troubleshooting Guide

## Database Connection Issues

### Issue: Npgsql Connection Errors

**Symptoms:**

- `Npgsql.NpgsqlException: Failed to connect to [host]:[port]`
- 500 errors on API endpoints
- Database timeout errors

**Solutions:**

### 1. Verify Database Configuration

```powershell
# Check if DATABASE_URL is set
railway variables

# Should show something like:
# DATABASE_URL=postgresql://username:password@host:port/database
```

### 2. Check Database Service Status

- Go to Railway dashboard
- Verify PostgreSQL service is running
- Check service logs for errors

### 3. Test Database Connection

```bash
# From Railway CLI
railway connect <database-service-name>
```

### 4. Environment Variables Checklist

Required variables:

- ✓ `DATABASE_URL` (automatically set by Railway PostgreSQL service)
- ✓ `ASPNETCORE_ENVIRONMENT=Production`
- ✓ `ASPNETCORE_URLS=http://0.0.0.0:$PORT`
- ✓ `JWT_SECRET_KEY` (32+ characters)
- ✓ `BREVO_API_KEY` (optional, for email service)

### 5. Common Connection String Issues

**Problem:** SSL connection failures
**Solution:** Updated connection string now includes:

- `SSL Mode=Require`
- `Trust Server Certificate=true`
- Connection timeout settings
- Retry logic

**Problem:** Invalid URL parsing
**Solution:** Improved URL parsing with error handling for both `DATABASE_URL` and `DATABASE_PUBLIC_URL`

## High Logging Rate Issues

### Issue: "Railway rate limit of 500 logs/sec reached"

**Symptoms:**

- Log message: "Messages dropped: [number]"
- Missing log entries
- Performance degradation

**Solutions:**

### 1. Reduced Log Levels (Already Implemented)

Production logging now set to:

- Default: Warning level
- Entity Framework: Error level only
- ASP.NET Core: Error level only
- Disabled sensitive data logging

### 2. Monitor Log Output

```powershell
# View real-time logs
railway logs --follow

# View specific number of recent logs
railway logs --tail 100
```

## Deployment Commands

### Initial Setup

```powershell
# 1. Set up environment variables
.\railway-setup.ps1

# 2. Deploy application
railway up

# 3. Monitor deployment
railway logs --follow
```

### Redeploy After Fixes

```powershell
# Build and deploy
railway up --detach

# Check deployment status
railway status

# View logs
railway logs --tail 50
```

## Health Check Verification

### Test Endpoints

1. **Health Check:** `https://your-app.railway.app/health`
2. **API Root:** `https://your-app.railway.app/`
3. **Swagger (disabled in production):** Not available

### Expected Health Check Response

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "duration": 50.5
    },
    {
      "name": "self",
      "status": "Healthy",
      "duration": 0.1
    }
  ],
  "totalDuration": 50.6,
  "timestamp": "2025-08-13T18:46:35.123Z"
}
```

## Database Migration Issues

### Issue: Migration fails during startup

**Solutions:**

1. **Check database permissions:**

   - Ensure user has CREATE permissions
   - Verify connection string credentials

2. **Manual migration (if needed):**

   ```powershell
   # Connect to Railway project
   railway login
   railway link

   # Run migration manually (if auto-migration fails)
   railway run dotnet ef database update
   ```

3. **Reset database (last resort):**
   - Delete and recreate PostgreSQL service in Railway
   - Redeploy application

## Common Error Patterns

### 1. Connection Timeout

```
Npgsql.NpgsqlException: Exception while connecting
```

**Fix:** Connection timeout increased to 30 seconds, retry logic added

### 2. SSL Certificate Issues

```
The remote certificate is invalid
```

**Fix:** Added `Trust Server Certificate=true` to connection string

### 3. Authentication Failed

```
password authentication failed for user
```

**Fix:** Verify DATABASE_URL credentials, check PostgreSQL service status

## Getting Help

### Debug Information to Collect

1. Railway logs: `railway logs --tail 100`
2. Environment variables: `railway variables`
3. Service status: `railway status`
4. Health check response: `curl https://your-app.railway.app/health`

### Support Resources

- Railway Documentation: https://docs.railway.app/
- Railway Discord: https://discord.gg/railway
- GitHub Issues: Create issue with debug information above
