using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace PersonalFinanceAPI.Infrastructure.Configuration;

/// <summary>
/// Swagger configuration and setup
/// </summary>
public static class SwaggerConfiguration
{
    /// <summary>
    /// Add Swagger configuration to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Personal Finance Management API",
                Version = "v1.0",
                Description = @"
## 🏆 Personal Finance Management API

A comprehensive, production-ready Personal Finance Management API built with .NET 8, Entity Framework Core, and PostgreSQL.

### 🚀 Key Features:
- 🔐 **JWT Authentication** with refresh tokens and secure password hashing
- 💰 **Transaction Management** with automatic categorization and CSV import
- 📊 **Budget Management** with real-time utilization tracking and alerts
- 📈 **Financial Analytics** with monthly summaries and spending insights
- 🎯 **Investment Suggestions** powered by intelligent algorithms
- 🚨 **Smart Alerts** for budget breaches and spending thresholds
- 🏦 **Bank Integration** simulation with account linking

### 🛠️ Technology Stack:
- **.NET 8** - Latest framework with performance optimizations
- **PostgreSQL** - Robust relational database with full ACID compliance
- **Entity Framework Core** - Modern ORM with advanced querying
- **JWT Authentication** - Secure, stateless authentication
- **Serilog** - Structured logging with multiple sinks
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Comprehensive input validation

### 🔐 Authentication:
All endpoints (except authentication endpoints) require a valid JWT token in the Authorization header.

**Format:** `Authorization: Bearer {your-jwt-token}`

### 📖 Getting Started:
1. **Register** a new user account using `/api/auth/register`
2. **Login** to receive JWT access and refresh tokens using `/api/auth/login`
3. **Import** transactions via CSV or create manually
4. **Create** budgets and set spending limits
5. **Monitor** your financial health through analytics

### 📊 Response Format:
All API responses follow a consistent format:
```json
{
  ""success"": true,
  ""data"": { /* response data */ },
  ""message"": ""Operation completed successfully"",
  ""timestamp"": ""2025-08-12T10:30:00Z""
}
```

### ⚠️ Error Handling:
Error responses include detailed information for debugging:
```json
{
  ""success"": false,
  ""message"": ""Validation failed"",
  ""details"": [
    {
      ""field"": ""email"",
      ""message"": ""Email is required""
    }
  ],
  ""correlationId"": ""abc123"",
  ""timestamp"": ""2025-08-12T10:30:00Z""
}
```

### 🏥 Health Checks:
Monitor API health at `/health` endpoint.
",
                Contact = new OpenApiContact
                {
                    Name = "Personal Finance API Team",
                    Email = "support@personalfinanceapi.com",
                    Url = new Uri("https://github.com/yashturmbekar1312/hackathon-dotnet-app")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                },
                TermsOfService = new Uri("https://personalfinanceapi.com/terms")
            });

            // JWT Authentication Security Definition
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"**JWT Authorization header using the Bearer scheme.**

Enter 'Bearer' [space] and then your token in the text input below.

**Example:** `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`

**How to get a token:**
1. Register a new account using `/api/auth/register`
2. Login using `/api/auth/login` to receive your JWT token
3. Copy the `accessToken` from the login response
4. Click the 'Authorize' button above and enter: `Bearer {your-token}`",
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

            // Include XML comments for documentation
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            // Enable annotations for better documentation
            c.EnableAnnotations();
            
            // Group endpoints by controller
            c.TagActionsBy(api => new[] { GetControllerTag(api) });
            c.DocInclusionPredicate((name, api) => true);

            // Add operation filters for enhanced documentation
            c.OperationFilter<SwaggerOperationFilter>();
            c.DocumentFilter<SwaggerDocumentFilter>();

            // Configure schemas
            c.SchemaFilter<SwaggerSchemaFilter>();

            // Add examples
            c.OperationFilter<SwaggerExamplesFilter>();
        });

        return services;
    }

    private static string GetControllerTag(ApiDescription api)
    {
        var controllerName = api.ActionDescriptor.RouteValues["controller"] ?? "Default";
        
        return controllerName switch
        {
            "Auth" => "🔐 Authentication",
            "Users" => "👤 User Management", 
            "Transactions" => "💸 Transactions",
            "Budgets" => "📊 Budget Management",
            "Analytics" => "📈 Financial Analytics",
            "Accounts" => "🏦 Account Management",
            _ => $"📋 {controllerName}"
        };
    }
}

/// <summary>
/// Operation filter for enhanced Swagger documentation
/// </summary>
public class SwaggerOperationFilter : IOperationFilter
{
    /// <summary>
    /// Apply operation filter to enhance Swagger documentation
    /// </summary>
    /// <param name="operation">OpenAPI operation</param>
    /// <param name="context">Operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add common response codes for all operations
        AddCommonResponses(operation);

        // Add security responses for protected endpoints
        AddSecurityResponses(operation, context);

        // Improve parameter descriptions
        ImproveParameterDescriptions(operation);

        // Add operation tags based on HTTP method
        AddOperationTags(operation, context);
    }

    private static void AddCommonResponses(OpenApiOperation operation)
    {
        // Add 500 Internal Server Error
        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.Add("500", new OpenApiResponse
            {
                Description = "Internal server error occurred",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["success"] = new OpenApiSchema { Type = "boolean", Example = new Microsoft.OpenApi.Any.OpenApiBoolean(false) },
                                ["message"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("An internal server error occurred") },
                                ["correlationId"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("abc123-def456") },
                                ["timestamp"] = new OpenApiSchema { Type = "string", Format = "date-time" }
                            }
                        }
                    }
                }
            });
        }

        // Add 400 Bad Request for POST/PUT operations
        if ((operation.OperationId?.Contains("Post") == true || operation.OperationId?.Contains("Put") == true) 
            && !operation.Responses.ContainsKey("400"))
        {
            operation.Responses.Add("400", new OpenApiResponse
            {
                Description = "Bad request - validation failed",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["success"] = new OpenApiSchema { Type = "boolean", Example = new Microsoft.OpenApi.Any.OpenApiBoolean(false) },
                                ["message"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("Validation failed") },
                                ["details"] = new OpenApiSchema 
                                { 
                                    Type = "array", 
                                    Items = new OpenApiSchema 
                                    { 
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["field"] = new OpenApiSchema { Type = "string" },
                                            ["message"] = new OpenApiSchema { Type = "string" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }
    }

    private static void AddSecurityResponses(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorizeAttribute = context.MethodInfo.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Any() ||
                                   context.MethodInfo.DeclaringType?.GetCustomAttributes<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().Any() == true;

        if (hasAuthorizeAttribute)
        {
            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add("401", new OpenApiResponse 
                { 
                    Description = "Unauthorized - Valid JWT token required in Authorization header" 
                });
            }

            if (!operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add("403", new OpenApiResponse 
                { 
                    Description = "Forbidden - Insufficient permissions for this operation" 
                });
            }
        }
    }

    private static void ImproveParameterDescriptions(OpenApiOperation operation)
    {
        foreach (var parameter in operation.Parameters ?? Enumerable.Empty<OpenApiParameter>())
        {
            if (parameter.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Description = "Unique identifier for the resource";
            }
            else if (parameter.Name.Contains("page", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Description = "Page number for pagination (1-based)";
            }
            else if (parameter.Name.Contains("size", StringComparison.OrdinalIgnoreCase) || 
                     parameter.Name.Contains("limit", StringComparison.OrdinalIgnoreCase))
            {
                parameter.Description = "Number of items per page (max 100)";
            }
        }
    }

    private static void AddOperationTags(OpenApiOperation operation, OperationFilterContext context)
    {
        var httpMethod = context.ApiDescription.HttpMethod?.ToUpperInvariant();
        var existing = operation.Summary ?? "";

        operation.Summary = httpMethod switch
        {
            "GET" => existing.StartsWith("Get") ? existing : $"Get {existing}".Trim(),
            "POST" => existing.StartsWith("Create") ? existing : $"Create {existing}".Trim(), 
            "PUT" => existing.StartsWith("Update") ? existing : $"Update {existing}".Trim(),
            "DELETE" => existing.StartsWith("Delete") ? existing : $"Delete {existing}".Trim(),
            _ => existing
        };
    }
}

/// <summary>
/// Document filter for global Swagger enhancements
/// </summary>
public class SwaggerDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Apply document filter to enhance Swagger documentation
    /// </summary>
    /// <param name="swaggerDoc">OpenAPI document</param>
    /// <param name="context">Document filter context</param>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // Add global tags
        swaggerDoc.Tags = new List<OpenApiTag>
        {
            new OpenApiTag { Name = "🔐 Authentication", Description = "User authentication and authorization endpoints" },
            new OpenApiTag { Name = "👤 User Management", Description = "User profile and account management" },
            new OpenApiTag { Name = "💸 Transactions", Description = "Financial transaction management and categorization" },
            new OpenApiTag { Name = "📊 Budget Management", Description = "Budget creation, monitoring, and alerts" },
            new OpenApiTag { Name = "📈 Financial Analytics", Description = "Financial insights, reports, and analytics" },
            new OpenApiTag { Name = "🏦 Account Management", Description = "Bank account linking and management" }
        };

        // Remove unused schemas
        var usedSchemas = new HashSet<string>();
        CollectUsedSchemas(swaggerDoc, usedSchemas);
        
        var schemasToRemove = swaggerDoc.Components.Schemas.Keys
            .Where(key => !usedSchemas.Contains(key))
            .ToList();
            
        foreach (var schema in schemasToRemove)
        {
            swaggerDoc.Components.Schemas.Remove(schema);
        }
    }

    private static void CollectUsedSchemas(OpenApiDocument document, HashSet<string> usedSchemas)
    {
        foreach (var path in document.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                CollectSchemasFromOperation(operation, usedSchemas);
            }
        }
    }

    private static void CollectSchemasFromOperation(OpenApiOperation operation, HashSet<string> usedSchemas)
    {
        // Collect from request body
        if (operation.RequestBody?.Content != null)
        {
            foreach (var content in operation.RequestBody.Content.Values)
            {
                CollectSchemasFromSchema(content.Schema, usedSchemas);
            }
        }

        // Collect from responses
        foreach (var response in operation.Responses.Values)
        {
            if (response.Content != null)
            {
                foreach (var content in response.Content.Values)
                {
                    CollectSchemasFromSchema(content.Schema, usedSchemas);
                }
            }
        }
    }

    private static void CollectSchemasFromSchema(OpenApiSchema? schema, HashSet<string> usedSchemas)
    {
        if (schema?.Reference?.Id != null)
        {
            usedSchemas.Add(schema.Reference.Id);
        }
    }
}

/// <summary>
/// Schema filter for improved model documentation
/// </summary>
public class SwaggerSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Apply schema filter to enhance model documentation
    /// </summary>
    /// <param name="schema">OpenAPI schema</param>
    /// <param name="context">Schema filter context</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.Name.EndsWith("Dto"))
        {
            schema.Description = $"Data transfer object for {context.Type.Name.Replace("Dto", "")} operations";
        }
        
        if (context.Type.Name.Contains("Request"))
        {
            schema.Description = $"Request model for {context.Type.Name.Replace("Request", "")} operations";
        }
        
        if (context.Type.Name.Contains("Response"))
        {
            schema.Description = $"Response model for {context.Type.Name.Replace("Response", "")} operations";
        }
    }
}

/// <summary>
/// Filter to add request/response examples
/// </summary>
public class SwaggerExamplesFilter : IOperationFilter
{
    /// <summary>
    /// Apply examples filter to add request/response examples
    /// </summary>
    /// <param name="operation">OpenAPI operation</param>
    /// <param name="context">Operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Add examples for common operations
        if (operation.OperationId?.Contains("Login") == true)
        {
            AddLoginExample(operation);
        }
        else if (operation.OperationId?.Contains("Register") == true)
        {
            AddRegisterExample(operation);
        }
    }

    private static void AddLoginExample(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content?.ContainsKey("application/json") == true)
        {
            operation.RequestBody.Content["application/json"].Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("user@example.com"),
                ["password"] = new Microsoft.OpenApi.Any.OpenApiString("SecurePassword123!")
            };
        }
    }

    private static void AddRegisterExample(OpenApiOperation operation)
    {
        if (operation.RequestBody?.Content?.ContainsKey("application/json") == true)
        {
            operation.RequestBody.Content["application/json"].Example = new Microsoft.OpenApi.Any.OpenApiObject
            {
                ["email"] = new Microsoft.OpenApi.Any.OpenApiString("newuser@example.com"),
                ["password"] = new Microsoft.OpenApi.Any.OpenApiString("SecurePassword123!"),
                ["firstName"] = new Microsoft.OpenApi.Any.OpenApiString("John"),
                ["lastName"] = new Microsoft.OpenApi.Any.OpenApiString("Doe"),
                ["phoneNumber"] = new Microsoft.OpenApi.Any.OpenApiString("+1234567890")
            };
        }
    }
}
