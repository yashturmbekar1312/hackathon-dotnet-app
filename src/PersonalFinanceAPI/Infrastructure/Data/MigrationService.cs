using Microsoft.EntityFrameworkCore;
using Serilog;

namespace PersonalFinanceAPI.Infrastructure.Data;

/// <summary>
/// Service responsible for handling database migrations automatically during application startup
/// </summary>
public static class MigrationService
{
    /// <summary>
    /// Applies pending migrations to the database and handles any migration errors gracefully
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the DbContext</param>
    /// <param name="retryCount">Number of times to retry migration on failure</param>
    /// <param name="delayBetweenRetries">Delay between retry attempts in milliseconds</param>
    /// <returns>True if migrations were applied successfully, false otherwise</returns>
    public static async Task<bool> ApplyMigrationsAsync(
        IServiceProvider serviceProvider, 
        int retryCount = 3, 
        int delayBetweenRetries = 5000)
    {
        for (int attempt = 1; attempt <= retryCount; attempt++)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                Log.Information("Checking database connectivity (Attempt {Attempt}/{MaxAttempts})", attempt, retryCount);
                
                // Check if database can be connected to
                var canConnect = await context.Database.CanConnectAsync();
                if (!canConnect)
                {
                    Log.Warning("Cannot connect to database on attempt {Attempt}", attempt);
                    if (attempt < retryCount)
                    {
                        await Task.Delay(delayBetweenRetries);
                        continue;
                    }
                    return false;
                }

                Log.Information("Database connection successful");

                // Get pending migrations
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                var pendingMigrationsList = pendingMigrations.ToList();
                
                if (pendingMigrationsList.Count == 0)
                {
                    Log.Information("No pending migrations found. Database is up to date");
                    return true;
                }

                Log.Information("Found {PendingCount} pending migration(s): {Migrations}", 
                    pendingMigrationsList.Count, 
                    string.Join(", ", pendingMigrationsList));

                // Apply migrations
                Log.Information("Applying database migrations...");
                await context.Database.MigrateAsync();
                
                Log.Information("Database migrations applied successfully");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply database migrations on attempt {Attempt}/{MaxAttempts}", attempt, retryCount);
                
                if (attempt == retryCount)
                {
                    Log.Fatal("All migration attempts failed. Database migration could not be completed");
                    return false;
                }
                
                Log.Information("Retrying migration in {DelayMs}ms...", delayBetweenRetries);
                await Task.Delay(delayBetweenRetries);
            }
        }
        
        return false;
    }

    /// <summary>
    /// Gets information about the current state of the database migrations
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the DbContext</param>
    /// <returns>Migration information including applied and pending migrations</returns>
    public static async Task<MigrationInfo> GetMigrationInfoAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var allMigrations = context.Database.GetMigrations();
            
            return new MigrationInfo
            {
                AppliedMigrations = appliedMigrations.ToList(),
                PendingMigrations = pendingMigrations.ToList(),
                AllMigrations = allMigrations.ToList(),
                IsUpToDate = !pendingMigrations.Any()
            };
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to get migration information");
            return new MigrationInfo
            {
                AppliedMigrations = new List<string>(),
                PendingMigrations = new List<string>(),
                AllMigrations = new List<string>(),
                IsUpToDate = false,
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Creates the database if it doesn't exist (for development scenarios)
    /// </summary>
    /// <param name="serviceProvider">The service provider to get the DbContext</param>
    /// <returns>True if database was created or already exists</returns>
    public static async Task<bool> EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var created = await context.Database.EnsureCreatedAsync();
            if (created)
            {
                Log.Information("Database created successfully");
            }
            else
            {
                Log.Information("Database already exists");
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to ensure database creation");
            return false;
        }
    }
}

/// <summary>
/// Information about the current state of database migrations
/// </summary>
public class MigrationInfo
{
    /// <summary>
    /// List of migrations that have been applied to the database
    /// </summary>
    public List<string> AppliedMigrations { get; set; } = new();
    
    /// <summary>
    /// List of migrations that are pending and need to be applied
    /// </summary>
    public List<string> PendingMigrations { get; set; } = new();
    
    /// <summary>
    /// List of all available migrations in the project
    /// </summary>
    public List<string> AllMigrations { get; set; } = new();
    
    /// <summary>
    /// Indicates whether the database is up to date with all migrations applied
    /// </summary>
    public bool IsUpToDate { get; set; }
    
    /// <summary>
    /// Error message if migration information retrieval failed
    /// </summary>
    public string? Error { get; set; }
}
