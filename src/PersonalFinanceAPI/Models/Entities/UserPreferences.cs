using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("user_preferences")]
public class UserPreferences
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("expense_threshold")]
    [Precision(12, 2)]
    public decimal ExpenseThreshold { get; set; } = 1000.00m;

    [Column("savings_goal_monthly")]
    [Precision(12, 2)]
    public decimal SavingsGoalMonthly { get; set; } = 5000.00m;

    [Column("investment_risk_level")]
    [MaxLength(20)]
    public string InvestmentRiskLevel { get; set; } = "MODERATE";

    [Column("notification_email")]
    public bool NotificationEmail { get; set; } = true;

    [Column("notification_sms")]
    public bool NotificationSms { get; set; } = false;

    [Column("notification_push")]
    public bool NotificationPush { get; set; } = true;

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "INR";

    [Column("timezone")]
    [MaxLength(50)]
    public string Timezone { get; set; } = "Asia/Kolkata";

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public virtual User User { get; set; } = null!;
}
