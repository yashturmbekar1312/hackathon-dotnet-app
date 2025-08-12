# Database Migration Guide

This guide explains how to handle Entity Framework migrations for the Personal Finance API, both for development and production deployments.

## Overview

The Personal Finance API uses Entity Framework Core with PostgreSQL and includes:

- ✅ Automatic migration application on startup
- ✅ Retry logic for migration failures
- ✅ Comprehensive logging for migration processes
- ✅ Deployment scripts for various environments

## Migration Files

The following migration-related files have been created:

### Core Migrations

- `Migrations/20250812180637_InitialCreate.cs` - Initial database schema migration
- `Migrations/20250812180637_InitialCreate.Designer.cs` - Migration metadata
- `Migrations/AppDbContextModelSnapshot.cs` - Current model snapshot

### Migration Services

- `Infrastructure/Data/MigrationService.cs` - Service for applying migrations with retry logic
- `migrate-database.sh` - Linux/macOS deployment script
- `migrate-database.ps1` - Windows PowerShell deployment script

## How Migrations Work

### 1. Automatic Migration on Startup

The application automatically applies pending migrations during startup:

```csharp
// In Program.cs
var migrationSuccess = await MigrationService.ApplyMigrationsAsync(
    app.Services,
    retryCount: 3,
    delayBetweenRetries: 5000);
```

**Features:**

- Retries failed migrations up to 3 times
- 5-second delay between retry attempts
- Comprehensive logging of migration status
- Graceful error handling

### 2. Production Deployment Options

You have multiple options for applying migrations in production:

#### Option A: Automatic Migration (Recommended)

The application will automatically apply migrations when it starts. This is the simplest approach and works well for most scenarios.

**Pros:**

- No manual intervention required
- Automatic retry logic
- Integrated with application lifecycle

**Cons:**

- Application startup time may be longer during migrations
- All app instances will attempt migration (first one wins)

#### Option B: Manual Migration via Scripts

Use the provided deployment scripts to apply migrations before starting the application.

**Linux/macOS:**

```bash
chmod +x migrate-database.sh
./migrate-database.sh
```

**Windows PowerShell:**

```powershell
.\migrate-database.ps1
```

**Pros:**

- Full control over migration timing
- Can be integrated into CI/CD pipelines
- Separates migration from application startup

#### Option C: Manual EF Commands

For advanced scenarios, use Entity Framework commands directly:

```bash
cd src/PersonalFinanceAPI
dotnet ef database update
```

## Environment Configuration

### Required Environment Variables

Ensure these environment variables are set:

```bash
# Primary database connection (Railway/Production)
DATABASE_PUBLIC_URL=postgresql://user:password@host:port/database

# Alternative connection string (if not using DATABASE_PUBLIC_URL)
ConnectionStrings__DefaultConnection=Host=localhost;Database=personalfinance;Username=user;Password=password
```

### Railway Deployment

For Railway deployment, the `DATABASE_PUBLIC_URL` is automatically provided. The application will:

1. Parse the Railway-provided database URL
2. Configure the connection string format for PostgreSQL
3. Apply migrations automatically on startup

### Local Development

For local development:

1. Set up your PostgreSQL database
2. Configure the connection string in `appsettings.Development.json`
3. The application will automatically apply migrations on first run

## Migration Commands

### Creating New Migrations

When you modify entity models, create a new migration:

```bash
cd src/PersonalFinanceAPI
dotnet ef migrations add <MigrationName>
```

Example:

```bash
dotnet ef migrations add AddUserProfileFields
```

### Viewing Migration Status

Check which migrations have been applied:

```bash
dotnet ef migrations list
```

### Rolling Back Migrations (Development Only)

To roll back to a specific migration:

```bash
dotnet ef database update <PreviousMigrationName>
```

**⚠️ Warning:** Never roll back migrations in production without careful planning.

### Removing the Last Migration (Development Only)

If you need to remove the last migration that hasn't been applied to production:

```bash
dotnet ef migrations remove
```

## Deployment Best Practices

### 1. CI/CD Integration

For automated deployments, integrate migration scripts into your pipeline:

```yaml
# Example GitHub Actions step
- name: Apply Database Migrations
  run: |
    chmod +x migrate-database.sh
    ./migrate-database.sh
  env:
    DATABASE_PUBLIC_URL: ${{ secrets.DATABASE_URL }}
```

### 2. Blue-Green Deployments

For zero-downtime deployments:

1. Apply migrations to a staging database first
2. Test the application with the new schema
3. Apply migrations to production
4. Deploy the new application version

### 3. Backup Strategy

Always backup your database before applying migrations in production:

```bash
# Example PostgreSQL backup
pg_dump $DATABASE_PUBLIC_URL > backup_$(date +%Y%m%d_%H%M%S).sql
```

## Troubleshooting

### Common Issues

#### 1. Connection Timeout During Migration

**Solution:** Increase the retry count and delay in the migration service or use manual migration scripts.

#### 2. Migration Lock Issues

**Solution:** If migrations get stuck, check for long-running transactions and restart the database if necessary.

#### 3. Seed Data Conflicts

**Solution:** The initial migration includes seed data. If you modify seed data, create a new migration rather than editing the initial one.

### Logging

Migration activities are logged with Serilog. Check the application logs for detailed migration information:

- Migration start/completion times
- Number of applied migrations
- Any errors or warnings during migration

### Health Checks

The application includes a health check endpoint that verifies database connectivity:

```
GET /health
```

This will show database status and can be used to verify successful migrations.

## Security Considerations

1. **Database Credentials:** Ensure database credentials are stored securely using environment variables or Azure Key Vault
2. **Migration Scripts:** Review migration scripts before applying to production
3. **Access Control:** Limit who can apply migrations in production environments
4. **Audit Trail:** Maintain logs of all migration activities for compliance

## Next Steps

1. ✅ Migrations are now set up and ready for deployment
2. ✅ The application will automatically handle database schema updates
3. ✅ Use the provided scripts for manual migration control if needed
4. 🔄 Create new migrations as you develop new features
5. 📊 Monitor migration performance and adjust retry settings if needed

For questions or issues with migrations, check the application logs and the troubleshooting section above.
