using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Infrastructure.Security;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Services;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Infrastructure.Configuration;
using PersonalFinanceAPI.Middleware;
using Serilog;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog early
LoggingConfiguration.ConfigureSerilog(builder.Configuration, builder.Environment);
builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Database Configuration
string connectionString;
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Parse DATABASE_PUBLIC_URL format: postgresql://user:password@host:port/database
    var uri = new Uri(databaseUrl);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No database connection string configured");
}

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// Enhanced JWT Authentication Configuration
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationPolicies();

// CORS Configuration
var corsSettings = builder.Configuration.GetSection("CORS");
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };
        var allowedMethods = corsSettings.GetSection("AllowedMethods").Get<string[]>() ?? new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
        var allowedHeaders = corsSettings.GetSection("AllowedHeaders").Get<string[]>() ?? new[] { "Content-Type", "Authorization" };

        if (allowedOrigins.Contains("*"))
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins);
        }

        policy.WithMethods(allowedMethods)
              .WithHeaders(allowedHeaders);
              
        if (!allowedOrigins.Contains("*"))
        {
            policy.AllowCredentials();
        }
    });
});

// Enhanced Swagger Configuration
if (builder.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger"))
{
    builder.Services.AddSwaggerConfiguration();
}

// Configuration Options
builder.Services.Configure<PersonalFinanceAPI.Models.DTOs.Email.EmailSettings>(options =>
{
    builder.Configuration.GetSection("Email").Bind(options);
    
    // Override ApiKey from environment variable if available
    var apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");
    if (!string.IsNullOrEmpty(apiKey))
    {
        options.ApiKey = apiKey;
    }
});

// FluentValidation Configuration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Service Registration
// Service Registration
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IEmailService, PersonalFinanceAPI.Application.Services.EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, PersonalFinanceAPI.Application.Services.UserService>();
builder.Services.AddScoped<IAccountService, PersonalFinanceAPI.Application.Services.AccountService>();
builder.Services.AddScoped<IAnalyticsService, PersonalFinanceAPI.Application.Services.AnalyticsService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();
builder.Services.AddScoped<IIncomePlanService, PersonalFinanceAPI.Application.Services.IncomePlanService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program));

// Enhanced Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // Use custom global exception middleware for production
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseHsts();
}

// Swagger Middleware
if (app.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Finance API v1.0");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DocumentTitle = "Personal Finance Management API";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}

// Enhanced Request Logging
app.UseEnhancedRequestLogging();

app.UseHttpsRedirection();

// CORS
app.UseCors("DefaultPolicy");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Enhanced Health Checks
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                exception = entry.Value.Exception?.Message,
                duration = entry.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow
        });
        await context.Response.WriteAsync(result);
    }
});

// Default route with API information
app.MapGet("/", () => new
{
    name = "Personal Finance Management API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    status = "healthy",
    documentation = app.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger") ? "Available at /" : "Disabled",
    healthCheck = "Available at /health"
});

// Global Exception Handler for Development
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<GlobalExceptionMiddleware>();
}

// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        Log.Information("Checking database connection...");
        await context.Database.EnsureCreatedAsync();
        
        Log.Information("Database connection established successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to connect to database during startup");
        throw;
    }
}

try
{
    Log.Information("Starting Personal Finance Management API");
    LoggingConfiguration.LogStartupInformation(app, app.Configuration);
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    LoggingConfiguration.LogShutdownInformation();
    Log.CloseAndFlush();
}

/// <summary>
/// Program entry point class for testing accessibility
/// </summary>
public partial class Program { }
