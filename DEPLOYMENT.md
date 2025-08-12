# 🚀 Production Deployment Guide

## 📋 Overview

This guide covers deploying the Personal Finance Management API to various cloud platforms using the `DATABASE_PUBLIC_URL` environment variable.

## 🔧 Environment Configuration

### Required Environment Variables

| Variable              | Description                  | Example                                   |
| --------------------- | ---------------------------- | ----------------------------------------- |
| `DATABASE_PUBLIC_URL` | PostgreSQL connection string | `postgresql://user:pass@host:5432/dbname` |
| `JWT_SECRET_KEY`      | Secret key for JWT tokens    | `your-super-secret-jwt-key-here`          |

### Optional Environment Variables

| Variable                 | Default      | Description                                   |
| ------------------------ | ------------ | --------------------------------------------- |
| `ASPNETCORE_ENVIRONMENT` | `Production` | Application environment                       |
| `FRONTEND_URL`           | `*`          | Frontend URL for CORS                         |
| `PORT`                   | `8080`       | Port number (Railway sets this automatically) |

---

## 🚂 Railway Deployment

### 1. Quick Deploy

[![Deploy on Railway](https://railway.app/button.svg)](https://railway.app/new/template?template=https://github.com/yourusername/hackathon-dotnet-app)

### 2. Manual Deployment

1. **Create Railway Project**

   ```bash
   railway login
   railway init
   railway add postgresql
   ```

2. **Set Environment Variables**

   ```bash
   railway variables set JWT_SECRET_KEY="your-super-secret-jwt-key-here"
   ```

3. **Deploy**
   ```bash
   railway up
   ```

### 3. Database Setup

Railway will automatically:

- Create a PostgreSQL database
- Set `DATABASE_URL` environment variable
- Our app uses this as `DATABASE_PUBLIC_URL`

---

## 🌊 Render Deployment

### 1. Web Service Setup

1. Connect your GitHub repository
2. Choose "Docker" as build environment
3. Set build command: `docker build -t app .`
4. Set start command: `docker run -p $PORT:8080 app`

### 2. Environment Variables

```
DATABASE_PUBLIC_URL=postgresql://user:pass@host:5432/dbname
JWT_SECRET_KEY=your-super-secret-jwt-key-here
ASPNETCORE_ENVIRONMENT=Production
```

### 3. Database Setup

1. Create PostgreSQL database in Render
2. Copy the External Database URL
3. Set as `DATABASE_PUBLIC_URL`

---

## 🐳 Docker Deployment Options

### Option 1: External Database (Recommended)

```bash
# Using external managed database
docker run -d \
  -p 8080:8080 \
  -e DATABASE_PUBLIC_URL="postgresql://user:pass@host:5432/dbname" \
  -e JWT_SECRET_KEY="your-secret-key" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  personalfinance-api
```

### Option 2: Local Development with Database

```bash
# Run with bundled PostgreSQL
docker-compose -f docker-compose.prod.yml --profile local up
```

### Option 3: Cloud Deployment

```bash
# Deploy-ready configuration
docker-compose -f docker-compose.deploy.yml up
```

---

## ☁️ AWS ECS Deployment

### 1. Task Definition

```json
{
  "family": "personalfinance-api",
  "containerDefinitions": [
    {
      "name": "api",
      "image": "your-ecr-repo/personalfinance-api:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "DATABASE_PUBLIC_URL",
          "value": "postgresql://user:pass@rds-endpoint:5432/dbname"
        },
        {
          "name": "JWT_SECRET_KEY",
          "value": "your-secret-key"
        },
        {
          "name": "ASPNETCORE_ENVIRONMENT",
          "value": "Production"
        }
      ],
      "healthCheck": {
        "command": [
          "CMD-SHELL",
          "curl -f http://localhost:8080/health || exit 1"
        ],
        "interval": 30,
        "timeout": 5,
        "retries": 3,
        "startPeriod": 60
      }
    }
  ]
}
```

### 2. RDS Setup

1. Create PostgreSQL RDS instance
2. Run database schema: `psql -f database-schema.sql`
3. Set `DATABASE_PUBLIC_URL` to RDS endpoint

---

## 🏗️ Azure Container Instances

### 1. Deploy Container

```bash
az container create \
  --resource-group myResourceGroup \
  --name personalfinance-api \
  --image your-registry/personalfinance-api:latest \
  --ports 8080 \
  --environment-variables \
    DATABASE_PUBLIC_URL="postgresql://user:pass@azure-postgres:5432/dbname" \
    JWT_SECRET_KEY="your-secret-key" \
    ASPNETCORE_ENVIRONMENT="Production"
```

### 2. Database Setup

1. Create Azure Database for PostgreSQL
2. Configure firewall rules
3. Run schema setup
4. Set connection string as `DATABASE_PUBLIC_URL`

---

## 🐙 GitHub Actions CI/CD

### 1. Workflow Example

```yaml
name: Deploy to Production

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3

      - name: Build Docker Image
        run: docker build -t personalfinance-api .

      - name: Deploy to Railway
        run: |
          npm install -g @railway/cli
          railway login --browserless
          railway up --detach
        env:
          RAILWAY_TOKEN: ${{ secrets.RAILWAY_TOKEN }}
```

### 2. Required Secrets

- `RAILWAY_TOKEN`
- `JWT_SECRET_KEY`
- `DATABASE_PUBLIC_URL` (if not using Railway PostgreSQL)

---

## 📊 Health Monitoring

### Health Check Endpoint

```
GET /health
Response: 200 OK
{
  "status": "Healthy",
  "timestamp": "2025-08-12T10:30:00Z"
}
```

### Application Metrics

- **Startup Time**: ~5-10 seconds
- **Memory Usage**: ~200-400 MB
- **Database Connections**: Pool of 10-20
- **Request Latency**: <100ms average

---

## 🔒 Security Configuration

### Production Checklist

- ✅ HTTPS enabled (handled by platform)
- ✅ JWT secret key set securely
- ✅ Database connection encrypted
- ✅ CORS properly configured
- ✅ Swagger disabled in production (optional)
- ✅ Error details hidden in production
- ✅ Request logging enabled

### Security Headers

The application automatically sets:

- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `X-XSS-Protection: 1; mode=block`

---

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Failed**

   ```
   Solution: Verify DATABASE_PUBLIC_URL format
   Format: postgresql://username:password@host:port/database
   ```

2. **JWT Token Issues**

   ```
   Solution: Ensure JWT_SECRET_KEY is set and consistent
   Minimum 32 characters recommended
   ```

3. **Port Binding Issues**

   ```
   Solution: Use PORT environment variable
   App binds to http://+:$PORT automatically
   ```

4. **CORS Errors**
   ```
   Solution: Set FRONTEND_URL or configure CORS__AllowedOrigins
   ```

### Debug Commands

```bash
# Check application logs
docker logs <container-name>

# Test health endpoint
curl http://your-app-url/health

# Test database connection
docker exec -it <container> psql $DATABASE_PUBLIC_URL -c "SELECT 1"
```

---

## 📈 Performance Tuning

### Database Optimization

- Connection pooling enabled (10-20 connections)
- EF Core query optimization
- Database indexes on frequently queried fields

### Application Optimization

- Async/await throughout
- Memory-efficient JSON serialization
- Request/response compression
- Static file caching

### Container Optimization

- Multi-stage Docker build
- Minimal base image (Alpine)
- Layer caching optimization
- Health checks for load balancing

---

## 🎯 Next Steps

1. **Set up monitoring** (Application Insights, Datadog, etc.)
2. **Configure alerts** for critical errors
3. **Set up backup strategy** for database
4. **Implement rate limiting** for API endpoints
5. **Add performance monitoring** for response times
6. **Set up log aggregation** (ELK stack, Splunk, etc.)

---

**🚀 Your Personal Finance Management API is now production-ready with `DATABASE_PUBLIC_URL` support!**
