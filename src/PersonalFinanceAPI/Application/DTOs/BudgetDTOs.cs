using System.ComponentModel.DataAnnotations;
using PersonalFinanceAPI.Models.Enums;

namespace PersonalFinanceAPI.Application.DTOs;

public class CreateBudgetRequest
{
    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0")]
    public decimal BudgetAmount { get; set; }

    [Required]
    public PeriodType PeriodType { get; set; } = PeriodType.MONTHLY;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateBudgetRequest
{
    public Guid? CategoryId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Budget amount must be greater than 0")]
    public decimal? BudgetAmount { get; set; }

    public PeriodType? PeriodType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }
}

public class BudgetResponse
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

public class BudgetUtilizationDto
{
    public Guid BudgetId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal BudgetAmount { get; set; }
    public decimal CurrentSpent { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public bool IsOverBudget { get; set; }
    public string PeriodType { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
