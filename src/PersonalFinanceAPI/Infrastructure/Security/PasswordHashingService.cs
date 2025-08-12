using BCrypt.Net;

namespace PersonalFinanceAPI.Infrastructure.Security;

public interface IPasswordHashingService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

public class PasswordHashingService : IPasswordHashingService
{
    private readonly ILogger<PasswordHashingService> _logger;

    public PasswordHashingService(ILogger<PasswordHashingService> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        try
        {
            // Use BCrypt with work factor of 12 for security
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, 12);
            _logger.LogDebug("Password hashed successfully");
            return hashedPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hashing password");
            throw;
        }
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            var isValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            _logger.LogDebug("Password verification result: {IsValid}", isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password");
            return false;
        }
    }
}
