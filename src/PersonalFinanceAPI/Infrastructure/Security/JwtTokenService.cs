using Microsoft.IdentityModel.Tokens;
using PersonalFinanceAPI.Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PersonalFinanceAPI.Infrastructure.Security;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    string GetJtiFromToken(string token);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured"));
        
        var jti = Guid.NewGuid().ToString();
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, jti),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("user_id", user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60")),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        _logger.LogInformation("Generated access token for user {UserId} with JTI {Jti}", user.Id, jti);
        
        return tokenString;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key not configured"))),
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate expired token");
            return null;
        }
    }

    public string GetJtiFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(token);
        return jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
    }
}
