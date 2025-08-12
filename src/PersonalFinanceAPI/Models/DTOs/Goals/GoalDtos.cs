using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Goals;

public class FinancialGoalDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string GoalType { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public int PriorityLevel { get; set; }
    public bool IsActive { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal RemainingAmount { get; set; }
    public int? DaysRemaining { get; set; }
    public decimal? MonthlyTargetAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateGoalRequest
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string GoalName { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(SAVINGS|INVESTMENT|DEBT_PAYOFF|EMERGENCY_FUND)$")]
    public string GoalType { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TargetAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CurrentAmount { get; set; } = 0;

    [DataType(DataType.Date)]
    public DateTime? TargetDate { get; set; }

    [Range(1, 5)]
    public int PriorityLevel { get; set; } = 3;
}

public class UpdateGoalRequest
{
    [StringLength(255, MinimumLength = 1)]
    public string? GoalName { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal? TargetAmount { get; set; }

    [DataType(DataType.Date)]
    public DateTime? TargetDate { get; set; }

    [Range(1, 5)]
    public int? PriorityLevel { get; set; }

    public bool? IsActive { get; set; }
}

public class ContributeToGoalRequest
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime? ContributionDate { get; set; }
}

public class GoalContributionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid GoalId { get; set; }
    public decimal ContributionAmount { get; set; }
    public decimal NewCurrentAmount { get; set; }
    public decimal NewProgressPercentage { get; set; }
    public bool GoalCompleted { get; set; }
    public DateTime ContributionDate { get; set; }
}

public class GoalProgressDto
{
    public Guid GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public string GoalType { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime? TargetDate { get; set; }
    public int? DaysRemaining { get; set; }
    public decimal? MonthlyTargetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int PriorityLevel { get; set; }
}

public class GoalInsightsDto
{
    public Guid GoalId { get; set; }
    public string GoalName { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public bool OnTrack { get; set; }
    public decimal MonthlyRequirement { get; set; }
    public decimal CurrentMonthlyAverage { get; set; }
    public DateTime? ProjectedCompletionDate { get; set; }
    public IEnumerable<string> Recommendations { get; set; } = new List<string>();
    public IEnumerable<GoalMilestoneDto> Milestones { get; set; } = new List<GoalMilestoneDto>();
    public IEnumerable<ContributionHistoryDto> RecentContributions { get; set; } = new List<ContributionHistoryDto>();
}

public class GoalMilestoneDto
{
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public bool IsAchieved { get; set; }
    public DateTime? AchievedDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class ContributionHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public decimal RunningTotal { get; set; }
}
