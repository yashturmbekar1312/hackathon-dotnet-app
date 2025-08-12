using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Infrastructure.Security;
using PersonalFinanceAPI.Models.DTOs.Auth;
using PersonalFinanceAPI.Models.Entities;
using PersonalFinanceAPI.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace PersonalFinanceAPI.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, string userAgent);
    Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, string userAgent);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, string userAgent);
    Task<bool> LogoutAsync(string refreshToken);
    Task<bool> RevokeAllTokensAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IEmailService _emailService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IJwtTokenService jwtTokenService,
        IPasswordHashingService passwordHashingService,
        IEmailService emailService,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _passwordHashingService = passwordHashingService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string ipAddress, string userAgent)
    {
        _logger.LogInformation("Starting user registration for email: {Email}", request.Email);

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("Registration attempted for existing email: {Email}", request.Email);
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create new user
        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHashingService.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            DateOfBirth = request.DateOfBirth,
            Occupation = request.Occupation,
            Currency = request.Currency,
            AnnualIncome = request.AnnualIncome,
            OtpCode = GenerateOtpCode(),
            OtpExpiresAt = DateTime.UtcNow.AddMinutes(10) // OTP expires in 10 minutes
        };

        _context.Users.Add(user);

        // Create default preferences
        var preferences = new UserPreferences
        {
            UserId = user.Id
        };

        _context.UserPreferences.Add(preferences);

        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var userSession = new UserSession
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            AccessTokenJti = _jwtTokenService.GetJtiFromToken(accessToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30), // Refresh token expires in 30 days
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.UserSessions.Add(userSession);
        await _context.SaveChangesAsync();

        // Send OTP via email
        try
        {
            var emailSent = await _emailService.SendOtpEmailAsync(user.Email, user.OtpCode, user.FirstName);
            if (emailSent)
            {
                _logger.LogInformation("OTP email sent successfully to user {UserId} at {Email}", user.Id, user.Email);
            }
            else
            {
                _logger.LogWarning("Failed to send OTP email to user {UserId} at {Email}, but registration continues", user.Id, user.Email);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending OTP email to user {UserId} at {Email}", user.Id, user.Email);
        }

        _logger.LogInformation("OTP generated for user {UserId}: {OtpCode} (expires at {ExpiresAt})", 
            user.Id, user.OtpCode, user.OtpExpiresAt);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = userSession.ExpiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
    {
        _logger.LogInformation("Verifying OTP for email: {Email}", request.Email);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            _logger.LogWarning("OTP verification attempted for non-existent email: {Email}", request.Email);
            return false;
        }

        if (user.OtpCode != request.OtpCode || user.OtpExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid or expired OTP for user {UserId}", user.Id);
            return false;
        }

        user.IsEmailVerified = true;
        user.OtpCode = null;
        user.OtpExpiresAt = null;

        await _context.SaveChangesAsync();

        // Send welcome email after verification
        try
        {
            var welcomeEmailSent = await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
            if (welcomeEmailSent)
            {
                _logger.LogInformation("Welcome email sent successfully to user {UserId}", user.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send welcome email to user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending welcome email to user {UserId}", user.Id);
        }

        _logger.LogInformation("Email verified successfully for user {UserId}", user.Id);
        return true;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string ipAddress, string userAgent)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);

        var user = await _context.Users
            .Include(u => u.Preferences)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);

        if (user == null || !_passwordHashingService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;

        // Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token
        var userSession = new UserSession
        {
            UserId = user.Id,
            RefreshToken = refreshToken,
            AccessTokenJti = _jwtTokenService.GetJtiFromToken(accessToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.UserSessions.Add(userSession);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged in successfully", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = userSession.ExpiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress, string userAgent)
    {
        _logger.LogInformation("Refreshing token for refresh token: {RefreshToken}", request.RefreshToken[..8] + "...");

        var userSession = await _context.UserSessions
            .Include(us => us.User)
            .ThenInclude(u => u.Preferences)
            .FirstOrDefaultAsync(us => us.RefreshToken == request.RefreshToken && 
                                     us.ExpiresAt > DateTime.UtcNow && 
                                     us.RevokedAt == null);

        if (userSession == null)
        {
            _logger.LogWarning("Invalid or expired refresh token");
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = userSession.User;

        if (!user.IsActive)
        {
            _logger.LogWarning("Refresh token attempt for inactive user {UserId}", user.Id);
            throw new UnauthorizedAccessException("User account is inactive");
        }

        // Revoke old session
        userSession.RevokedAt = DateTime.UtcNow;

        // Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Create new session
        var newUserSession = new UserSession
        {
            UserId = user.Id,
            RefreshToken = newRefreshToken,
            AccessTokenJti = _jwtTokenService.GetJtiFromToken(accessToken),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            IpAddress = ipAddress,
            UserAgent = userAgent
        };

        _context.UserSessions.Add(newUserSession);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Token refreshed successfully for user {UserId}", user.Id);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = newUserSession.ExpiresAt,
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken)
    {
        _logger.LogInformation("Logging out user with refresh token: {RefreshToken}", refreshToken[..8] + "...");

        var userSession = await _context.UserSessions
            .FirstOrDefaultAsync(us => us.RefreshToken == refreshToken && us.RevokedAt == null);

        if (userSession == null)
        {
            _logger.LogWarning("Logout attempted with invalid refresh token");
            return false;
        }

        userSession.RevokedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} logged out successfully", userSession.UserId);
        return true;
    }

    public async Task<bool> RevokeAllTokensAsync(Guid userId)
    {
        _logger.LogInformation("Revoking all tokens for user {UserId}", userId);

        var userSessions = await _context.UserSessions
            .Where(us => us.UserId == userId && us.RevokedAt == null)
            .ToListAsync();

        foreach (var session in userSessions)
        {
            session.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Revoked {Count} tokens for user {UserId}", userSessions.Count, userId);
        return true;
    }

    private string GenerateOtpCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var code = Math.Abs(BitConverter.ToInt32(bytes, 0)) % 1000000;
        return code.ToString("D6");
    }

    private UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            Occupation = user.Occupation,
            Currency = user.Currency,
            AnnualIncome = user.AnnualIncome,
            IsEmailVerified = user.IsEmailVerified,
            IsPhoneVerified = user.IsPhoneVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }
}
