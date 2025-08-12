using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Suggestions;

public class InvestmentSuggestionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SuggestionType { get; set; } = string.Empty;
    public string InvestmentProduct { get; set; } = string.Empty;
    public decimal SuggestedAmount { get; set; }
    public decimal? ExpectedReturnRate { get; set; }
    public string? RiskLevel { get; set; }
    public string? Reasoning { get; set; }
    public int PriorityScore { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSuggestionRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string SuggestionType { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string InvestmentProduct { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal SuggestedAmount { get; set; }

    [Range(0, 100)]
    public decimal? ExpectedReturnRate { get; set; }

    [RegularExpression("^(LOW|MODERATE|HIGH)$")]
    public string? RiskLevel { get; set; }

    public string? Reasoning { get; set; }

    [Range(1, 100)]
    public int PriorityScore { get; set; } = 50;

    public DateTime? ExpiresAt { get; set; }
}

public class AcceptSuggestionRequest
{
    public string? Notes { get; set; }
    public decimal? InvestmentAmount { get; set; }
}

public class RejectSuggestionRequest
{
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class PostponeSuggestionRequest
{
    public DateTime? PostponeUntil { get; set; }
    public string? Reason { get; set; }
}

public class SuggestionActionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid SuggestionId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
}

public class SuggestionHistoryDto
{
    public Guid Id { get; set; }
    public Guid SuggestionId { get; set; }
    public Guid UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime ActionDate { get; set; }
    public string? Notes { get; set; }
    public InvestmentSuggestionDto? Suggestion { get; set; }
}

public class SuggestionStats
{
    public int TotalSuggestions { get; set; }
    public int AcceptedSuggestions { get; set; }
    public int RejectedSuggestions { get; set; }
    public int PendingSuggestions { get; set; }
    public decimal TotalSuggestedAmount { get; set; }
    public decimal AcceptedAmount { get; set; }
    public double AcceptanceRate { get; set; }
}
