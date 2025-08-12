using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Alerts;

public class UserAlertDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsActionable { get; set; }
    public string? ActionUrl { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
}

public class CreateAlertRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string AlertType { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [RegularExpression("^(INFO|WARNING|CRITICAL)$")]
    public string Severity { get; set; } = "INFO";

    public bool IsActionable { get; set; } = false;

    [Url]
    public string? ActionUrl { get; set; }

    public Dictionary<string, object>? Metadata { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

public class AlertPreferencesDto
{
    public Guid UserId { get; set; }
    public bool BudgetBreachAlerts { get; set; }
    public bool ExpenseThresholdAlerts { get; set; }
    public bool LowSavingsAlerts { get; set; }
    public bool InvestmentSuggestionAlerts { get; set; }
    public bool GoalMilestoneAlerts { get; set; }
    public bool MonthlyReportAlerts { get; set; }
    public bool EmailNotifications { get; set; }
    public bool SmsNotifications { get; set; }
    public bool PushNotifications { get; set; }
    public TimeSpan QuietHoursStart { get; set; }
    public TimeSpan QuietHoursEnd { get; set; }
}

public class UpdateAlertPreferencesRequest
{
    public bool? BudgetBreachAlerts { get; set; }
    public bool? ExpenseThresholdAlerts { get; set; }
    public bool? LowSavingsAlerts { get; set; }
    public bool? InvestmentSuggestionAlerts { get; set; }
    public bool? GoalMilestoneAlerts { get; set; }
    public bool? MonthlyReportAlerts { get; set; }
    public bool? EmailNotifications { get; set; }
    public bool? SmsNotifications { get; set; }
    public bool? PushNotifications { get; set; }
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
}
