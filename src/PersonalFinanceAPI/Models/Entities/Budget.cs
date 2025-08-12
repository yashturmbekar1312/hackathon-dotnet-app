using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("budgets")]
public class Budget
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("category_id")]
    public Guid CategoryId { get; set; }

    [Required]
    [Column("budget_amount")]
    [Precision(12, 2)]
    public decimal BudgetAmount { get; set; }

    [Required]
    [Column("period_type")]
    [MaxLength(20)]
    public string PeriodType { get; set; } = "MONTHLY"; // WEEKLY, MONTHLY, QUARTERLY, YEARLY

    [Required]
    [Column("start_date")]
    public DateOnly StartDate { get; set; }

    [Required]
    [Column("end_date")]
    public DateOnly EndDate { get; set; }

    [Column("current_spent")]
    [Precision(12, 2)]
    public decimal CurrentSpent { get; set; } = 0.00m;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<BudgetUtilization> Utilizations { get; set; } = new List<BudgetUtilization>();

    // Helper properties
    [NotMapped]
    public decimal RemainingAmount => BudgetAmount - CurrentSpent;

    [NotMapped]
    public decimal UtilizationPercentage => BudgetAmount > 0 ? (CurrentSpent / BudgetAmount) * 100 : 0;

    [NotMapped]
    public bool IsOverBudget => CurrentSpent > BudgetAmount;
}
