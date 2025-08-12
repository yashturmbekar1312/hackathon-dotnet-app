using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("linked_accounts")]
public class LinkedAccount
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("bank_aggregator_id")]
    public Guid? BankAggregatorId { get; set; }

    [Required]
    [Column("account_name")]
    [MaxLength(255)]
    public string AccountName { get; set; } = string.Empty;

    [Column("account_number")]
    [MaxLength(50)]
    public string? AccountNumber { get; set; }

    [Required]
    [Column("account_type")]
    [MaxLength(20)]
    public string AccountType { get; set; } = string.Empty; // SAVINGS, CURRENT, CREDIT_CARD, INVESTMENT

    [Required]
    [Column("bank_name")]
    [MaxLength(255)]
    public string BankName { get; set; } = string.Empty;

    [Column("balance")]
    [Precision(15, 2)]
    public decimal Balance { get; set; } = 0.00m;

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "INR";

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("last_synced_at")]
    public DateTime? LastSyncedAt { get; set; }

    [Column("external_account_id")]
    [MaxLength(255)]
    public string? ExternalAccountId { get; set; }

    [Column("consent_given_at")]
    public DateTime ConsentGivenAt { get; set; } = DateTime.UtcNow;

    [Column("consent_expires_at")]
    public DateTime? ConsentExpiresAt { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual BankAggregator? BankAggregator { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Transaction> TransferTransactions { get; set; } = new List<Transaction>();
}
