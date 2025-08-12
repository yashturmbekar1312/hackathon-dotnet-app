using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("first_name")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Column("phone_number")]
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Column("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [Column("occupation")]
    [MaxLength(100)]
    public string? Occupation { get; set; }

    [Column("currency")]
    [MaxLength(3)]
    public string Currency { get; set; } = "INR";

    [Column("annual_income")]
    [Precision(18, 2)]
    public decimal? AnnualIncome { get; set; }

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; } = false;

    [Column("is_phone_verified")]
    public bool IsPhoneVerified { get; set; } = false;

    [Column("otp_code")]
    [MaxLength(6)]
    public string? OtpCode { get; set; }

    [Column("otp_expires_at")]
    public DateTime? OtpExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [Column("last_login_at")]
    public DateTime? LastLoginAt { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual UserPreferences? Preferences { get; set; }
    public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
    public virtual ICollection<LinkedAccount> LinkedAccounts { get; set; } = new List<LinkedAccount>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    public virtual ICollection<UserAlert> Alerts { get; set; } = new List<UserAlert>();
    public virtual ICollection<MonthlySummary> MonthlySummaries { get; set; } = new List<MonthlySummary>();
    public virtual ICollection<FinancialGoal> FinancialGoals { get; set; } = new List<FinancialGoal>();
    public virtual ICollection<InvestmentSuggestion> InvestmentSuggestions { get; set; } = new List<InvestmentSuggestion>();
    public virtual ICollection<IncomePlan> IncomePlans { get; set; } = new List<IncomePlan>();
}
