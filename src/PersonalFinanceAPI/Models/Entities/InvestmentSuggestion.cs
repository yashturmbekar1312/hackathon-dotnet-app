using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("investment_suggestions")]
public class InvestmentSuggestion
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("suggestion_type")]
    [MaxLength(50)]
    public string SuggestionType { get; set; } = string.Empty;

    [Required]
    [Column("investment_product")]
    [MaxLength(255)]
    public string InvestmentProduct { get; set; } = string.Empty;

    [Required]
    [Column("suggested_amount")]
    [Precision(15, 2)]
    public decimal SuggestedAmount { get; set; }

    [Column("expected_return_rate")]
    [Precision(5, 2)]
    public decimal? ExpectedReturnRate { get; set; }

    [Column("risk_level")]
    [MaxLength(20)]
    public string? RiskLevel { get; set; } // LOW, MODERATE, HIGH

    [Column("reasoning")]
    public string? Reasoning { get; set; }

    [Column("priority_score")]
    public int PriorityScore { get; set; } = 50;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<SuggestionHistory> History { get; set; } = new List<SuggestionHistory>();
}
