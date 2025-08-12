using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Models.DTOs.Auth;
using PersonalFinanceAPI.Services;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var response = await _authService.RegisterAsync(request, ipAddress, userAgent);

            _logger.LogInformation("User registered successfully with email: {Email}", request.Email);

            return Ok(new
            {
                success = true,
                message = "User registered successfully. Please verify your email with the OTP sent.",
                data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration for email: {Email}", request.Email);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during registration"
            });
        }
    }

    /// <summary>
    /// Verify email with OTP code
    /// </summary>
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        try
        {
            var result = await _authService.VerifyOtpAsync(request);

            if (result)
            {
                _logger.LogInformation("Email verified successfully for: {Email}", request.Email);
                return Ok(new
                {
                    success = true,
                    message = "Email verified successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Invalid or expired OTP code"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during OTP verification for email: {Email}", request.Email);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during OTP verification"
            });
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var response = await _authService.LoginAsync(request, ipAddress, userAgent);

            _logger.LogInformation("User logged in successfully with email: {Email}", request.Email);

            return Ok(new
            {
                success = true,
                message = "Login successful",
                data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return Unauthorized(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during login"
            });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var response = await _authService.RefreshTokenAsync(request, ipAddress, userAgent);

            _logger.LogInformation("Token refreshed successfully");

            return Ok(new
            {
                success = true,
                message = "Token refreshed successfully",
                data = response
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during token refresh"
            });
        }
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.LogoutAsync(request.RefreshToken);

            if (result)
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("User {UserId} logged out successfully", userId);

                return Ok(new
                {
                    success = true,
                    message = "Logout successful"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Invalid refresh token"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred during logout"
            });
        }
    }

    /// <summary>
    /// Revoke all refresh tokens for the current user
    /// </summary>
    [HttpPost("revoke-all-tokens")]
    [Authorize]
    public async Task<IActionResult> RevokeAllTokens()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authService.RevokeAllTokensAsync(userId);

            if (result)
            {
                _logger.LogInformation("All tokens revoked for user {UserId}", userId);

                return Ok(new
                {
                    success = true,
                    message = "All tokens revoked successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = "Failed to revoke tokens"
            });
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "Error revoking tokens for user {UserId}", userId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while revoking tokens"
            });
        }
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            return Ok(new
            {
                success = true,
                data = new
                {
                    id = userId,
                    email = userEmail,
                    name = userName
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user information");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while getting user information"
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

    private string GetClientIpAddress()
    {
        var ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
        }
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        return ipAddress ?? "Unknown";
    }
}
