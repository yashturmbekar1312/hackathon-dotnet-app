using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities;

[Table("merchants")]
public class Merchant
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Column("mcc_code")]
    [MaxLength(4)]
    public string? MccCode { get; set; }

    [Column("is_verified")]
    public bool IsVerified { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Category? Category { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
