using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalFinanceAPI.Models.Entities;

[Table("user_alerts")]
public class UserAlert
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("alert_type")]
    [MaxLength(50)]
    public string AlertType { get; set; } = string.Empty;

    [Required]
    [Column("title")]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [Column("message")]
    public string Message { get; set; } = string.Empty;

    [Column("severity")]
    [MaxLength(20)]
    public string Severity { get; set; } = "INFO"; // INFO, WARNING, CRITICAL

    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("is_actionable")]
    public bool IsActionable { get; set; } = false;

    [Column("action_url")]
    [MaxLength(255)]
    public string? ActionUrl { get; set; }

    [Column("metadata", TypeName = "jsonb")]
    public string? MetadataJson { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("acknowledged_at")]
    public DateTime? AcknowledgedAt { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    // Helper property for metadata
    [NotMapped]
    public JsonDocument? Metadata
    {
        get => string.IsNullOrEmpty(MetadataJson) ? null : JsonDocument.Parse(MetadataJson);
        set => MetadataJson = value?.RootElement.GetRawText();
    }
}
