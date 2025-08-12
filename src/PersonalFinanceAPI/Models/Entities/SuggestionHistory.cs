using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalFinanceAPI.Models.Entities;

[Table("suggestion_history")]
public class SuggestionHistory
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("suggestion_id")]
    public Guid SuggestionId { get; set; }

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("action")]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty; // VIEWED, ACCEPTED, REJECTED, POSTPONED

    [Column("action_date")]
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;

    [Column("notes")]
    public string? Notes { get; set; }

    // Navigation properties
    public virtual InvestmentSuggestion Suggestion { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
