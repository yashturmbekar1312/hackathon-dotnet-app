using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceAPI.Infrastructure.Data;

namespace PersonalFinanceAPI.Tests.Infrastructure;

/// <summary>
/// Custom web application factory for integration tests
/// </summary>
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (ApplicationDbContext)
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            try
            {
                // Seed the database with test data
                SeedTestData(db);
            }
            catch (Exception ex)
            {
                // Log errors or do anything you think it's needed
                throw new Exception($"An error occurred seeding the database with test data. Error: {ex.Message}");
            }
        });

        builder.UseEnvironment("Testing");
    }

    private static void SeedTestData(AppDbContext context)
    {
        // Add test data here
        // This method will be called to seed the in-memory database with test data
        
        // Example: Add test users, transactions, etc.
        // context.Users.Add(new User { ... });
        // context.SaveChanges();
    }
}
