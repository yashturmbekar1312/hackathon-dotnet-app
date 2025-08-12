using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("monthly_summaries")]
public class MonthlySummary
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("month_year")]
    public DateOnly MonthYear { get; set; }

    [Column("total_income")]
    [Precision(15, 2)]
    public decimal TotalIncome { get; set; } = 0.00m;

    [Column("total_expenses")]
    [Precision(15, 2)]
    public decimal TotalExpenses { get; set; } = 0.00m;

    [Column("net_savings")]
    [Precision(15, 2)]
    public decimal NetSavings { get; set; } = 0.00m;

    [Column("top_expense_category_id")]
    public Guid? TopExpenseCategoryId { get; set; }

    [Column("top_expense_amount")]
    [Precision(15, 2)]
    public decimal TopExpenseAmount { get; set; } = 0.00m;

    [Column("transaction_count")]
    public int TransactionCount { get; set; } = 0;

    [Column("average_transaction_amount")]
    [Precision(15, 2)]
    public decimal AverageTransactionAmount { get; set; } = 0.00m;

    [Column("savings_rate")]
    [Precision(5, 2)]
    public decimal SavingsRate { get; set; } = 0.00m;

    [Column("is_final")]
    public bool IsFinal { get; set; } = false;

    [Column("computed_at")]
    public DateTime ComputedAt { get; set; } = DateTime.UtcNow;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Category? TopExpenseCategory { get; set; }
}
