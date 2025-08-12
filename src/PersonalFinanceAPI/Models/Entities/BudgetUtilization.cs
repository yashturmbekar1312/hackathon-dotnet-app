using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("budget_utilizations")]
public class BudgetUtilization
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("budget_id")]
    public Guid BudgetId { get; set; }

    [Required]
    [Column("transaction_id")]
    public Guid TransactionId { get; set; }

    [Required]
    [Column("amount_utilized")]
    [Precision(12, 2)]
    public decimal AmountUtilized { get; set; }

    [Required]
    [Column("utilization_date")]
    public DateOnly UtilizationDate { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Budget Budget { get; set; } = null!;
    public virtual Transaction Transaction { get; set; } = null!;
}
