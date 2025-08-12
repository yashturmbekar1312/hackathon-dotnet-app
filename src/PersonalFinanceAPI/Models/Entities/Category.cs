using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities;

[Table("categories")]
public class Category
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("parent_id")]
    public Guid? ParentId { get; set; }

    [Required]
    [Column("type")]
    [MaxLength(20)]
    public string Type { get; set; } = string.Empty; // INCOME, EXPENSE, TRANSFER

    [Column("icon")]
    [MaxLength(50)]
    public string? Icon { get; set; }

    [Column("color")]
    [MaxLength(7)]
    public string? Color { get; set; }

    [Column("is_system_defined")]
    public bool IsSystemDefined { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; } = new List<Category>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();
}
