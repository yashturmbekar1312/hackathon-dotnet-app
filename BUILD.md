# 🚀 Build and Run Instructions

## Quick Start (Local Development)

### 1. Prerequisites Check
```bash
# Check .NET version
dotnet --version
# Should be 8.0.x

# Check PostgreSQL
psql --version
# Should be 14+

# Check Docker (optional)
docker --version
```

### 2. Database Setup (Option A - Docker)
```bash
# Start PostgreSQL with schema
docker-compose up postgres -d

# Wait for container to be ready (30 seconds)
# Check logs: docker-compose logs postgres
```

### 3. Database Setup (Option B - Local PostgreSQL)
```bash
# Create database
createdb personal_finance_dev

# Import schema
psql -d personal_finance_dev -f database-schema.sql

# Verify tables
psql -d personal_finance_dev -c "\dt"
```

### 4. Configure Application
```bash
# Update appsettings.Development.json if needed
# Default connection should work with Docker setup
```

### 5. Run Application
```bash
cd src/PersonalFinanceAPI
dotnet restore
dotnet build
dotnet run
```

### 6. Verify API
```bash
# Health check
curl http://localhost:8080/health

# Swagger UI
open http://localhost:8080
```

### 7. Test API (Automated)
```bash
# Make script executable (Linux/Mac)
chmod +x test-api.sh

# Run comprehensive test
./test-api.sh
```

## Production Deployment (Docker)

### 1. Build Production Image
```bash
docker build -t personalfinance-api .
```

### 2. Run with Production Config
```bash
docker-compose -f docker-compose.prod.yml up -d
```

### 3. Environment Variables for Production
```bash
export DATABASE_PASSWORD="your-secure-password"
export JWT_SECRET_KEY="your-super-secure-jwt-key-at-least-32-characters"
export ASPNETCORE_ENVIRONMENT="Production"
```

## Railway Deployment

### 1. Install Railway CLI
```bash
npm install -g @railway/cli
railway login
```

### 2. Deploy to Railway
```bash
# Initialize project
railway init

# Add PostgreSQL
railway add postgresql

# Set environment variables
railway variables set JWT_SECRET_KEY="your-production-jwt-secret"
railway variables set ASPNETCORE_ENVIRONMENT="Production"

# Deploy
railway up
```

## Troubleshooting

### Database Connection Issues
```bash
# Check PostgreSQL status
sudo systemctl status postgresql
# or for Docker:
docker-compose logs postgres

# Test connection
psql -h localhost -p 5432 -U postgres -d personal_finance_dev
```

### API Not Starting
```bash
# Check logs
dotnet run --verbosity diagnostic

# Common issues:
# - Port 8080 already in use
# - Database connection failed
# - JWT secret key not configured
```

### Docker Issues
```bash
# Check containers
docker-compose ps

# View logs
docker-compose logs api
docker-compose logs postgres

# Restart services
docker-compose restart
```

## Sample Data

The application automatically includes:
- **12 default categories** (Food, Transport, etc.)
- **6 sample merchants** (Swiggy, Uber, etc.)
- **2 bank aggregators** for testing
- **Sample transactions** generated on account sync

## API Testing

### Manual Testing (cURL)
```bash
# Register user
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!","firstName":"John","lastName":"Doe"}'

# Login
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test123!"}'

# Use JWT token for authenticated requests
curl -X GET http://localhost:8080/api/analytics/dashboard \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### Using Swagger UI
1. Open http://localhost:8080
2. Register a new user
3. Login to get JWT token
4. Click "Authorize" and enter: `Bearer YOUR_JWT_TOKEN`
5. Test all endpoints interactively

## Performance Notes

- Database queries are optimized with proper indexing
- Pagination is implemented for large result sets
- Connection pooling is enabled for database efficiency
- Caching can be added with Redis for production

## Security Features

- JWT authentication with refresh tokens
- Password hashing with BCrypt
- Rate limiting on sensitive endpoints
- Input validation with FluentValidation
- SQL injection protection with parameterized queries
- CORS protection
- Audit logging for compliance

## Monitoring

### Health Checks
```bash
curl http://localhost:8080/health
# Returns: {"status":"Healthy","totalDuration":"00:00:00.0123456"}
```

### Logs
```bash
# View application logs
tail -f logs/personal-finance-api-*.log

# Docker logs
docker-compose logs -f api
```

## Feature Highlights

✅ **Complete User Journey**: Register → Verify → Login → Link Account → Import Transactions → Create Budgets → View Analytics

✅ **Production Ready**: Error handling, logging, validation, security, documentation

✅ **Scalable Architecture**: Clean separation of concerns, dependency injection, async/await

✅ **Modern .NET 8**: Latest framework features and best practices

✅ **Cloud Ready**: Docker support, environment configuration, health checks

---

**Ready to impress the judges! 🏆**
