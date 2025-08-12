using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Controllers;
using PersonalFinanceAPI.Tests.Infrastructure;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace PersonalFinanceAPI.Tests.Integration.Controllers;

/// <summary>
/// Integration tests for AuthController
/// </summary>
public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "newuser@example.com",
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("success");
        content.Should().Contain("User registered successfully");
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "invalid-email",
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "user@example.com",
            Password = "123", // Weak password
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var email = "logintest@example.com";
        var password = "SecurePassword123!";

        // First register a user
        var registerRequest = new
        {
            Email = email,
            Password = password,
            FirstName = "Login",
            LastName = "Test",
            PhoneNumber = "+1234567890"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("accessToken");
        content.Should().Contain("refreshToken");
        content.Should().Contain("success");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        var token = await GetTestUserTokenAsync();
        SetAuthenticationToken(token);

        // Act
        var response = await Client.GetAsync("/api/users/profile");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound); // Depending on your implementation
    }

    [Fact]
    public async Task RefreshToken_WithValidRefreshToken_ShouldReturnNewTokens()
    {
        // Arrange
        var email = "refreshtest@example.com";
        var password = "SecurePassword123!";

        // Register and login to get initial tokens
        var registerRequest = new
        {
            Email = email,
            Password = password,
            FirstName = "Refresh",
            LastName = "Test",
            PhoneNumber = "+1234567890"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        var loginRequest = new { Email = email, Password = password };
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginResult = await DeserializeResponse<dynamic>(loginResponse);
        
        var refreshToken = loginResult?.GetProperty("refreshToken").GetString();
        refreshToken.Should().NotBeNullOrEmpty();

        var refreshRequest = new { RefreshToken = refreshToken };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("accessToken");
        content.Should().Contain("refreshToken");
    }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnOk()
    {
        // Arrange
        var token = await GetTestUserTokenAsync("logouttest@example.com");
        SetAuthenticationToken(token);

        // Act
        var response = await Client.PostAsync("/api/auth/logout", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    public async Task Register_WithInvalidEmailFormats_ShouldReturnBadRequest(string invalidEmail)
    {
        // Arrange
        var registerRequest = new
        {
            Email = invalidEmail,
            Password = "SecurePassword123!",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("password")]
    [InlineData("PASSWORD")]
    [InlineData("12345678")]
    public async Task Register_WithInvalidPasswords_ShouldReturnBadRequest(string invalidPassword)
    {
        // Arrange
        var registerRequest = new
        {
            Email = "user@example.com",
            Password = invalidPassword,
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
