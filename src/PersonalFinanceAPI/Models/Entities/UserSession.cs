using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities;

[Table("user_sessions")]
public class UserSession
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("refresh_token")]
    [MaxLength(512)]
    public string RefreshToken { get; set; } = string.Empty;

    [Required]
    [Column("access_token_jti")]
    [MaxLength(255)]
    public string AccessTokenJti { get; set; } = string.Empty;

    [Column("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    // Navigation property
    public virtual User User { get; set; } = null!;
}
