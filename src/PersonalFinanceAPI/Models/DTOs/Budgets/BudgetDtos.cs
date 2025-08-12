using System.ComponentModel.DataAnnotations;
using PersonalFinanceAPI.Models.DTOs.Transactions;

namespace PersonalFinanceAPI.Models.DTOs.Budgets;

public class BudgetDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal CurrentSpent { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public bool IsOverBudget { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateBudgetRequest
{
    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal BudgetAmount { get; set; }

    [Required]
    public string PeriodType { get; set; } = "MONTHLY";

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}

public class UpdateBudgetRequest
{
    [Range(0.01, double.MaxValue)]
    public decimal? BudgetAmount { get; set; }

    public string? PeriodType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }
}

public class BudgetSummaryDto
{
    public decimal TotalBudgetAmount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public decimal OverallUtilizationPercentage { get; set; }
    public int TotalBudgets { get; set; }
    public int OverBudgetCount { get; set; }
    public List<BudgetDto> Budgets { get; set; } = new();
    public List<BudgetAlertDto> Alerts { get; set; } = new();
}

public class BudgetAlertDto
{
    public Guid BudgetId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal CurrentSpent { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public string AlertType { get; set; } = string.Empty; // WARNING, EXCEEDED
    public string Message { get; set; } = string.Empty;
}

public class BudgetUtilizationDto
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public Guid TransactionId { get; set; }
    public decimal AmountUtilized { get; set; }
    public DateOnly UtilizationDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public TransactionDto? Transaction { get; set; }
}

public class BudgetQueryParameters
{
    public Guid? CategoryId { get; set; }
    public string? PeriodType { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsOverBudget { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "CreatedAt";
    public string SortOrder { get; set; } = "desc";
}
