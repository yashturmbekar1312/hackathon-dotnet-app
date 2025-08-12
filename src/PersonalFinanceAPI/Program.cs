using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Infrastructure.Security;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Services;
using PersonalFinanceAPI.Core.Interfaces;
using Serilog;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("JWT Challenge: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

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
              .WithHeaders(allowedHeaders)
              .AllowCredentials();
    });
});

// Swagger Configuration
if (builder.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger"))
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Personal Finance Management API",
            Version = "v1",
            Description = "A comprehensive API for personal finance management with features including transaction tracking, budget management, savings computation, investment suggestions, and analytics.",
            Contact = new OpenApiContact
            {
                Name = "Personal Finance API Team",
                Email = "support@personalfinanceapi.com",
                Url = new Uri("https://github.com/personalfinanceapi")
            }
        });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // Include XML comments
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

// FluentValidation Configuration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Service Registration
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IBudgetService, BudgetService>();

// AutoMapper Configuration
builder.Services.AddAutoMapper(typeof(Program));

// Health Checks
builder.Services.AddHealthChecks();

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
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Swagger Middleware
if (app.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Finance API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
        c.DocumentTitle = "Personal Finance Management API";
        c.DefaultModelsExpandDepth(-1);
        c.DisplayRequestDuration();
    });
}

// Request Logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? Serilog.Events.LogEventLevel.Error
        : httpContext.Response.StatusCode > 499
            ? Serilog.Events.LogEventLevel.Error
            : Serilog.Events.LogEventLevel.Information;
});

app.UseHttpsRedirection();

// CORS
app.UseCors("DefaultPolicy");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Controllers
app.MapControllers();

// Health Checks
app.MapHealthChecks("/health");

// Default route
app.MapGet("/", () => new
{
    name = "Personal Finance Management API",
    version = "1.0.0",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow,
    status = "healthy"
});

// Global Exception Handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;
            Log.Error(ex, "Unhandled exception occurred");

            var response = new
            {
                success = false,
                message = app.Environment.IsDevelopment() ? ex.Message : "An internal server error occurred",
                details = app.Environment.IsDevelopment() ? ex.StackTrace : null
            };

            await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
        }
    });
});

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
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    Log.Information("Swagger UI available at: {SwaggerUrl}", app.Configuration.GetValue<bool>("FeatureFlags:EnableSwagger") ? "/" : "Disabled");
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
