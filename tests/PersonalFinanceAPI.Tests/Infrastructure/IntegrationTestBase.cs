using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PersonalFinanceAPI.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;

namespace PersonalFinanceAPI.Tests.Infrastructure;

/// <summary>
/// Base class for integration tests
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory<Program>>
{
    protected readonly HttpClient Client;
    protected readonly CustomWebApplicationFactory<Program> Factory;
    protected readonly AppDbContext DbContext;

    protected IntegrationTestBase(CustomWebApplicationFactory<Program> factory)
    {
        Factory = factory;
        Client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// Create HTTP content from object
    /// </summary>
    protected static StringContent CreateJsonContent(object obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    /// <summary>
    /// Deserialize HTTP response content
    /// </summary>
    protected static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// Set authentication token for requests
    /// </summary>
    protected void SetAuthenticationToken(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Clear authentication token
    /// </summary>
    protected void ClearAuthenticationToken()
    {
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Get JWT token for test user
    /// </summary>
    protected async Task<string> GetTestUserTokenAsync(string email = "test@example.com", string password = "TestPassword123!")
    {
        // First register the user
        var registerRequest = new
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890"
        };

        await Client.PostAsync("/api/auth/register", CreateJsonContent(registerRequest));

        // Then login to get token
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var loginResponse = await Client.PostAsync("/api/auth/login", CreateJsonContent(loginRequest));
        var loginResult = await DeserializeResponse<dynamic>(loginResponse);
        
        // Extract token from response (this will depend on your actual response structure)
        // You may need to adjust this based on your AuthController's response format
        return loginResult?.GetProperty("accessToken").GetString() ?? throw new Exception("Failed to get test token");
    }

    /// <summary>
    /// Clean up database after each test
    /// </summary>
    protected virtual void Cleanup()
    {
        // Remove test data
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }
}
