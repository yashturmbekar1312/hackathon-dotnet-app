# 🚂 Railway Deployment Instructions

## Quick Deploy to Railway

Railway will automatically detect your Dockerfile and deploy your .NET application. No `railway.toml` needed!

### 1. Prerequisites
- Railway account ([railway.app](https://railway.app))
- Railway CLI installed (optional)

### 2. Deploy via GitHub (Recommended)

1. **Connect Repository**
   - Go to [railway.app](https://railway.app)
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your `hackathon-dotnet-app` repository

2. **Add PostgreSQL Database**
   - In your Railway project dashboard
   - Click "New Service"
   - Select "Database" → "PostgreSQL"
   - Railway will automatically set `DATABASE_URL`

3. **Configure Environment Variables**
   ```
   JWT_SECRET_KEY=your-super-secret-jwt-key-minimum-32-characters
   ASPNETCORE_ENVIRONMENT=Production
   ```

4. **Deploy**
   - Railway will automatically build and deploy
   - Your app will be available at `https://your-app-name.railway.app`

### 3. Deploy via CLI (Alternative)

```bash
# Install Railway CLI
npm install -g @railway/cli

# Login to Railway
railway login

# Initialize project
railway init

# Add PostgreSQL database
railway add postgresql

# Set environment variables
railway variables set JWT_SECRET_KEY="your-super-secret-jwt-key-minimum-32-characters"

# Deploy
railway up
```

### 4. Environment Variables Setup

Railway automatically provides:
- ✅ `DATABASE_URL` - PostgreSQL connection string
- ✅ `PORT` - Application port (Railway sets this)

You need to set:
- 🔑 `JWT_SECRET_KEY` - Your secret key for JWT tokens

Optional:
- 🌐 `FRONTEND_URL` - Your frontend URL for CORS
- ⚙️ `ASPNETCORE_ENVIRONMENT` - Set to "Production"

### 5. Database Schema Setup

After first deployment, run the database schema:

```bash
# Option 1: Use Railway CLI
railway connect postgresql
\i database-schema.sql

# Option 2: Use psql directly
psql $DATABASE_URL -f database-schema.sql
```

### 6. Verify Deployment

Your API will be available at:
- **Base URL**: `https://your-app-name.railway.app`
- **Health Check**: `https://your-app-name.railway.app/health`
- **API Docs**: `https://your-app-name.railway.app/` (Swagger UI)

### 7. Monitoring

Railway provides:
- 📊 **Logs**: Real-time application logs
- 📈 **Metrics**: CPU, memory, network usage
- 🔄 **Deployments**: Deployment history and rollbacks
- 💾 **Database**: PostgreSQL monitoring and backups

---

## ✅ Railway Configuration Summary

Railway automatically detects and configures:
- ✅ **Dockerfile** - Builds your .NET application
- ✅ **Port Binding** - Uses `$PORT` environment variable
- ✅ **Database** - PostgreSQL with `DATABASE_URL`
- ✅ **HTTPS** - Automatic SSL certificate
- ✅ **Domain** - Provides `railway.app` subdomain
- ✅ **Scaling** - Automatic horizontal scaling
- ✅ **Health Checks** - Uses `/health` endpoint

No `railway.toml` configuration file needed! 🎉

---

## 🐛 Troubleshooting

**Issue**: Deployment fails
- **Solution**: Check logs in Railway dashboard
- **Common**: Ensure `Dockerfile` is in repository root

**Issue**: Database connection fails  
- **Solution**: Verify PostgreSQL service is running
- **Check**: `DATABASE_URL` environment variable is set

**Issue**: JWT authentication fails
- **Solution**: Ensure `JWT_SECRET_KEY` is set
- **Requirement**: Minimum 32 characters

**Issue**: CORS errors
- **Solution**: Set `FRONTEND_URL` or configure CORS origins

---

## 🎯 Next Steps After Deployment

1. ✅ Test all API endpoints
2. ✅ Verify database connectivity
3. ✅ Check health endpoint
4. ✅ Test authentication flow
5. ✅ Monitor application logs
6. ✅ Set up custom domain (optional)
7. ✅ Configure scaling policies

**Your Personal Finance API is now live on Railway! 🚀**
