using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace PersonalFinanceAPI.Models.Entities;

[Table("audit_logs")]
public class AuditLog
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Required]
    [Column("entity_type")]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [Column("entity_id")]
    public Guid? EntityId { get; set; }

    [Required]
    [Column("action")]
    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [Column("old_values", TypeName = "jsonb")]
    public string? OldValuesJson { get; set; }

    [Column("new_values", TypeName = "jsonb")]
    public string? NewValuesJson { get; set; }

    [Column("ip_address")]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    public string? UserAgent { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User? User { get; set; }

    // Helper properties for JSON data
    [NotMapped]
    public JsonDocument? OldValues
    {
        get => string.IsNullOrEmpty(OldValuesJson) ? null : JsonDocument.Parse(OldValuesJson);
        set => OldValuesJson = value?.RootElement.GetRawText();
    }

    [NotMapped]
    public JsonDocument? NewValues
    {
        get => string.IsNullOrEmpty(NewValuesJson) ? null : JsonDocument.Parse(NewValuesJson);
        set => NewValuesJson = value?.RootElement.GetRawText();
    }
}
