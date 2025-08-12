using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("financial_goals")]
public class FinancialGoal
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("goal_name")]
    [MaxLength(255)]
    public string GoalName { get; set; } = string.Empty;

    [Required]
    [Column("goal_type")]
    [MaxLength(50)]
    public string GoalType { get; set; } = string.Empty; // SAVINGS, INVESTMENT, DEBT_PAYOFF, EMERGENCY_FUND

    [Required]
    [Column("target_amount")]
    [Precision(15, 2)]
    public decimal TargetAmount { get; set; }

    [Column("current_amount")]
    [Precision(15, 2)]
    public decimal CurrentAmount { get; set; } = 0.00m;

    [Column("target_date")]
    public DateOnly? TargetDate { get; set; }

    [Column("priority_level")]
    public int PriorityLevel { get; set; } = 3;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;

    // Helper properties
    [NotMapped]
    public decimal RemainingAmount => TargetAmount - CurrentAmount;

    [NotMapped]
    public decimal ProgressPercentage => TargetAmount > 0 ? (CurrentAmount / TargetAmount) * 100 : 0;

    [NotMapped]
    public bool IsCompleted => CurrentAmount >= TargetAmount;
}
