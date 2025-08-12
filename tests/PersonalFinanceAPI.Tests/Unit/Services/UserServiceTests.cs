using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PersonalFinanceAPI.Application.Services;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.Entities;
using PersonalFinanceAPI.Models.DTOs.Users;
using PersonalFinanceAPI.Core.Exceptions;
using Xunit;

namespace PersonalFinanceAPI.Tests.Unit.Services;

/// <summary>
/// Unit tests for UserService
/// </summary>
public class UserServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _userService;
    private readonly Fixture _fixture;

    public UserServiceTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<UserService>>();
        _userService = new UserService(_context, _mockLogger.Object);
        _fixture = new Fixture();
        
        // Configure AutoFixture to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task GetUserProfileAsync_WithExistingUser_ShouldReturnUserProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "1234567890",
            DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-25)),
            IsEmailVerified = true,
            IsPhoneVerified = false,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            IsActive = true
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Email.Should().Be("test@example.com");
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetUserProfileAsync_WithNonExistingUser_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserProfileAsync(userId));
    }

    [Fact]
    public async Task UpdateUserProfileAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var updateRequest = new UpdateUserProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            PhoneNumber = "9876543210",
            DateOfBirth = DateTime.Now.AddYears(-30)
        };

        // Act
        var result = await _userService.UpdateUserProfileAsync(userId, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
        result.PhoneNumber.Should().Be("9876543210");
    }

    [Fact]
    public async Task GetUserPreferencesAsync_WithNewUser_ShouldCreateDefaultPreferences()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userService.GetUserPreferencesAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.CurrencyCode.Should().Be("INR");
        result.Timezone.Should().Be("Asia/Kolkata");
        result.NotificationEmail.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUserAccountAsync_WithExistingUser_ShouldSoftDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userService.DeleteUserAccountAsync(userId);

        // Assert
        result.Should().BeTrue();
        
        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        deletedUser.Should().NotBeNull();
        deletedUser!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAccountAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userService.DeleteUserAccountAsync(userId);

        // Assert
        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}