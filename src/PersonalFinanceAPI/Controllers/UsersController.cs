using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Core.Exceptions;
using PersonalFinanceAPI.Models.DTOs.Users;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile information
    /// </summary>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _userService.GetUserProfileAsync(userId);

            return Ok(new
            {
                success = true,
                data = profile
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving user profile"
            });
        }
    }

    /// <summary>
    /// Update user profile information
    /// </summary>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var profile = await _userService.UpdateUserProfileAsync(userId, request);

            _logger.LogInformation("Profile updated for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "Profile updated successfully",
                data = profile
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "User not found for profile update");
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating profile"
            });
        }
    }

    /// <summary>
    /// Get user preferences and settings
    /// </summary>
    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferences()
    {
        try
        {
            var userId = GetCurrentUserId();
            var preferences = await _userService.GetUserPreferencesAsync(userId);

            return Ok(new
            {
                success = true,
                data = preferences
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user preferences");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving preferences"
            });
        }
    }

    /// <summary>
    /// Update user preferences and settings
    /// </summary>
    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferences([FromBody] UpdateUserPreferencesRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var preferences = await _userService.UpdateUserPreferencesAsync(userId, request);

            _logger.LogInformation("Preferences updated for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "Preferences updated successfully",
                data = preferences
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user preferences");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating preferences"
            });
        }
    }

    /// <summary>
    /// Update savings thresholds and goals
    /// </summary>
    [HttpPut("savings-thresholds")]
    public async Task<IActionResult> UpdateSavingsThresholds([FromBody] UpdateSavingsThresholdsRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var preferences = await _userService.UpdateSavingsThresholdsAsync(userId, request);

            _logger.LogInformation("Savings thresholds updated for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "Savings thresholds updated successfully",
                data = preferences
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating savings thresholds");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating savings thresholds"
            });
        }
    }

    /// <summary>
    /// Delete user account (soft delete)
    /// </summary>
    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userService.DeleteUserAccountAsync(userId);

            if (result)
            {
                _logger.LogInformation("Account deleted for user {UserId}", userId);

                return Ok(new
                {
                    success = true,
                    message = "Account deleted successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Failed to delete account"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user account");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while deleting account"
            });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("user_id")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Invalid user context");
    }
}
