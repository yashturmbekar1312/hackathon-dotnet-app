using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using System.Diagnostics;

namespace PersonalFinanceAPI.Infrastructure.Configuration;

/// <summary>
/// Serilog logging configuration
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Configure Serilog logger with environment-specific settings
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="environment">Web host environment</param>
    public static void ConfigureSerilog(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithProperty("Application", "PersonalFinanceAPI")
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .Enrich.WithProperty("ProcessId", Environment.ProcessId);

        // Console logging with colored output
        loggerConfig.WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            restrictedToMinimumLevel: environment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Information);

        // File logging with rolling intervals
        var logPath = configuration["Serilog:WriteTo:1:Args:path"] ?? "logs/personal-finance-api-.log";
        loggerConfig.WriteTo.File(
            path: logPath,
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: environment.IsDevelopment() ? 7 : 30,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}",
            restrictedToMinimumLevel: LogEventLevel.Information);

        // Error-only file logging
        loggerConfig.WriteTo.File(
            path: "logs/errors/personal-finance-api-errors-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 90,
            restrictedToMinimumLevel: LogEventLevel.Error,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{SourceContext}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        // Performance logging for slow operations
        loggerConfig.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.WithProperty("ElapsedMilliseconds"))
            .WriteTo.File(
                path: "logs/performance/personal-finance-api-performance-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Message:lj} - Duration: {ElapsedMilliseconds}ms {Properties:j}{NewLine}"));

        // Security audit logging
        loggerConfig.WriteTo.Logger(lc => lc
            .Filter.ByIncludingOnly(Matching.WithProperty("AuditEvent"))
            .WriteTo.File(
                path: "logs/audit/personal-finance-api-audit-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 365, // Keep audit logs for a year
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] AUDIT: {Message:lj} {Properties:j}{NewLine}"));

        // Production-specific logging to external services
        if (environment.IsProduction())
        {
            // Add structured logging for production monitoring
            // Example: Application Insights, Seq, Elasticsearch, etc.
            var instrumentationKey = configuration["ApplicationInsights:InstrumentationKey"];
            if (!string.IsNullOrEmpty(instrumentationKey))
            {
                // loggerConfig.WriteTo.ApplicationInsights(instrumentationKey, TelemetryConverter.Traces);
            }
        }

        // Filter out noisy logs in production
        if (!environment.IsDevelopment())
        {
            loggerConfig.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Hosting.Diagnostics"));
            loggerConfig.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Mvc.Infrastructure"));
            loggerConfig.Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.Routing.EndpointMiddleware"));
        }

        Log.Logger = loggerConfig.CreateLogger();
    }

    /// <summary>
    /// Add request logging configuration
    /// </summary>
    /// <param name="app">Web application</param>
    /// <returns>Web application</returns>
    public static WebApplication UseEnhancedRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode >= 400
                        ? LogEventLevel.Warning
                        : elapsed > 5000
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault());
                diagnosticContext.Set("ClientIP", GetClientIPAddress(httpContext));
                diagnosticContext.Set("ContentType", httpContext.Request.ContentType);
                diagnosticContext.Set("RequestSize", httpContext.Request.ContentLength);
                diagnosticContext.Set("ResponseSize", httpContext.Response.ContentLength);
                
                if (httpContext.User.Identity?.IsAuthenticated == true)
                {
                    diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ?? 
                                                   httpContext.User.FindFirst("id")?.Value);
                    diagnosticContext.Set("UserEmail", httpContext.User.FindFirst("email")?.Value);
                }

                if (httpContext.Request.Query.Any())
                {
                    diagnosticContext.Set("QueryParameters", httpContext.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString()));
                }

                // Add correlation ID if present
                if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
                {
                    diagnosticContext.Set("CorrelationId", correlationId.FirstOrDefault());
                }
            };
        });

        return app;
    }

    /// <summary>
    /// Log application startup information
    /// </summary>
    /// <param name="app">Web application</param>
    /// <param name="configuration">Application configuration</param>
    public static void LogStartupInformation(WebApplication app, IConfiguration configuration)
    {
        var logger = Log.ForContext<Program>();
        
        logger.Information("=== Personal Finance API Starting ===");
        logger.Information("Environment: {Environment}", app.Environment.EnvironmentName);
        logger.Information("Application Version: {Version}", GetApplicationVersion());
        logger.Information(".NET Version: {NetVersion}", Environment.Version);
        logger.Information("Machine Name: {MachineName}", Environment.MachineName);
        logger.Information("Process ID: {ProcessId}", Environment.ProcessId);
        logger.Information("Working Directory: {WorkingDirectory}", Environment.CurrentDirectory);
        
        // Log configuration settings (without sensitive data)
        logger.Information("Database Provider: PostgreSQL");
        logger.Information("JWT Issuer: {JwtIssuer}", configuration["Jwt:Issuer"]);
        logger.Information("CORS Origins: {CorsOrigins}", string.Join(", ", configuration.GetSection("CORS:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>()));
        logger.Information("Swagger Enabled: {SwaggerEnabled}", configuration.GetValue<bool>("FeatureFlags:EnableSwagger"));
        logger.Information("Rate Limiting Enabled: {RateLimitingEnabled}", configuration.GetValue<bool>("FeatureFlags:EnableRateLimiting"));
        
        var urls = configuration["ASPNETCORE_URLS"] ?? "http://localhost:5000";
        logger.Information("Listening on: {Urls}", urls);
        
        logger.Information("=== Startup Complete ===");
    }

    /// <summary>
    /// Log application shutdown information
    /// </summary>
    public static void LogShutdownInformation()
    {
        var logger = Log.ForContext<Program>();
        logger.Information("=== Personal Finance API Shutting Down ===");
        logger.Information("Shutdown Time: {ShutdownTime}", DateTime.UtcNow);
        logger.Information("Uptime: {Uptime}", DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime());
        logger.Information("=== Shutdown Complete ===");
    }

    private static string GetClientIPAddress(HttpContext httpContext)
    {
        // Check for forwarded headers (when behind reverse proxy)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIP = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIP))
        {
            return realIP;
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static string GetApplicationVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version?.ToString() ?? "1.0.0.0";
    }
}

/// <summary>
/// Custom logger extensions for structured logging
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Log audit events for security and compliance
    /// </summary>
    public static void LogAudit(this Microsoft.Extensions.Logging.ILogger logger, string action, object? additionalData = null)
    {
        logger.LogInformation("AUDIT: {Action} {Data} {AuditEvent}", 
            action, 
            additionalData, 
            true);
    }

    /// <summary>
    /// Log performance metrics
    /// </summary>
    public static void LogPerformance(this Microsoft.Extensions.Logging.ILogger logger, string operation, long elapsedMilliseconds, object? additionalData = null)
    {
        logger.LogInformation("PERFORMANCE: {Operation} completed in {ElapsedMilliseconds}ms {Data}", 
            operation, 
            elapsedMilliseconds, 
            additionalData);
    }

    /// <summary>
    /// Log business events
    /// </summary>
    public static void LogBusinessEvent(this Microsoft.Extensions.Logging.ILogger logger, string eventName, object? eventData = null)
    {
        logger.LogInformation("BUSINESS_EVENT: {EventName} {EventData}", eventName, eventData);
    }

    /// <summary>
    /// Log security events
    /// </summary>
    public static void LogSecurity(this Microsoft.Extensions.Logging.ILogger logger, string securityEvent, object? securityData = null)
    {
        logger.LogWarning("SECURITY: {SecurityEvent} {SecurityData}", securityEvent, securityData);
    }
}
