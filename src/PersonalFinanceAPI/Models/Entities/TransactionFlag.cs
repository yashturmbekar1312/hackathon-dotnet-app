using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities;

[Table("transaction_flags")]
public class TransactionFlag
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; }

    [Required]
    [Column("flag_type")]
    [MaxLength(50)]
    public string FlagType { get; set; } = string.Empty;

    [Column("flag_value")]
    [MaxLength(255)]
    public string? FlagValue { get; set; }

    [Column("created_by")]
    public Guid? CreatedBy { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Transaction Transaction { get; set; } = null!;
    public virtual User? CreatedByUser { get; set; }
}
