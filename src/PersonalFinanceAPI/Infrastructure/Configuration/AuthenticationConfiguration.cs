using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PersonalFinanceAPI.Infrastructure.Security;

namespace PersonalFinanceAPI.Infrastructure.Configuration;

/// <summary>
/// Authentication and authorization configuration
/// </summary>
public static class AuthenticationConfiguration
{
    /// <summary>
    /// Add JWT authentication to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = !IsDevelopment();
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
                ClockSkew = TimeSpan.Zero, // Remove default 5-minute tolerance
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // Enhanced JWT events for logging and monitoring
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    var userId = context.Principal?.FindFirst("sub")?.Value ?? context.Principal?.FindFirst("id")?.Value;
                    
                    logger.LogInformation("JWT token validated successfully for user {UserId}", userId);
                    
                    // Add user context for subsequent use
                    context.HttpContext.Items["UserId"] = userId;
                    context.HttpContext.Items["UserEmail"] = context.Principal?.FindFirst("email")?.Value;
                    
                    return Task.CompletedTask;
                },

                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    
                    logger.LogWarning("JWT authentication failed: {Error}. Token: {Token}", 
                        context.Exception.Message, 
                        context.Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "").Substring(0, 10) + "...");
                    
                    // Set custom response for authentication failures
                    context.Response.Headers["WWW-Authenticate"] = "Bearer error=\"invalid_token\"";
                    
                    return Task.CompletedTask;
                },

                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    
                    logger.LogWarning("JWT challenge occurred: {Error}, Description: {ErrorDescription}", 
                        context.Error, 
                        context.ErrorDescription);
                    
                    // Customize challenge response
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        success = false,
                        message = "Authentication required. Please provide a valid JWT token in the Authorization header.",
                        details = new[]
                        {
                            "Format: Authorization: Bearer {your-jwt-token}",
                            "Obtain a token by logging in via /api/auth/login"
                        },
                        statusCode = 401,
                        timestamp = DateTime.UtcNow
                    };
                    
                    return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                },

                OnForbidden = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    var userId = context.Principal?.FindFirst("sub")?.Value ?? "Unknown";
                    
                    logger.LogWarning("Access forbidden for user {UserId} to {Path}", userId, context.Request.Path);
                    
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Add authorization policies
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Default policy requires authenticated user
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Admin policy
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("role", "Admin"));

            // Premium user policy
            options.AddPolicy("PremiumUser", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("subscription", "Premium", "Enterprise"));

            // Account owner policy (can access own resources)
            options.AddPolicy("AccountOwner", policy =>
                policy.RequireAuthenticatedUser()
                      .AddRequirements(new AccountOwnerRequirement()));

            // Data export policy (sensitive operations)
            options.AddPolicy("DataExport", policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permissions", "data:export")
                      .RequireAssertion(context => 
                          DateTime.UtcNow.Hour >= 9 && DateTime.UtcNow.Hour <= 17)); // Business hours only
        });

        // Register custom authorization handlers
        services.AddScoped<Microsoft.AspNetCore.Authorization.IAuthorizationHandler, AccountOwnerAuthorizationHandler>();

        return services;
    }

    private static bool IsDevelopment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
    }
}

/// <summary>
/// Custom requirement for account ownership
/// </summary>
public class AccountOwnerRequirement : Microsoft.AspNetCore.Authorization.IAuthorizationRequirement
{
}

/// <summary>
/// Authorization handler for account ownership
/// </summary>
public class AccountOwnerAuthorizationHandler : Microsoft.AspNetCore.Authorization.AuthorizationHandler<AccountOwnerRequirement>
{
    /// <summary>
    /// Handle the account ownership requirement
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">Account owner requirement</param>
    /// <returns>Task</returns>
    protected override Task HandleRequirementAsync(
        Microsoft.AspNetCore.Authorization.AuthorizationHandlerContext context, 
        AccountOwnerRequirement requirement)
    {
        var userId = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("id")?.Value;
        
        if (context.Resource is HttpContext httpContext)
        {
            // Check if user is accessing their own resources
            var routeUserId = httpContext.Request.RouteValues["userId"]?.ToString();
            var queryUserId = httpContext.Request.Query["userId"].FirstOrDefault();
            
            if (userId == routeUserId || userId == queryUserId || string.IsNullOrEmpty(routeUserId))
            {
                context.Succeed(requirement);
            }
        }
        
        return Task.CompletedTask;
    }
}
