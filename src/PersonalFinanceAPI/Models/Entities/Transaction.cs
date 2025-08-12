using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceAPI.Models.Entities;

[Table("transactions")]
public class Transaction
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Required]
    [Column("linked_account_id")]
    public Guid LinkedAccountId { get; set; }

    [Column("merchant_id")]
    public Guid? MerchantId { get; set; }

    [Column("category_id")]
    public Guid? CategoryId { get; set; }

    [Required]
    [Column("amount")]
    [Precision(15, 2)]
    public decimal Amount { get; set; }

    [Column("currency_code")]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "INR";

    [Required]
    [Column("transaction_type")]
    [MaxLength(20)]
    public string TransactionType { get; set; } = string.Empty; // DEBIT, CREDIT

    [Column("description")]
    public string? Description { get; set; }

    [Column("reference_number")]
    [MaxLength(255)]
    public string? ReferenceNumber { get; set; }

    [Required]
    [Column("transaction_date")]
    public DateOnly TransactionDate { get; set; }

    [Column("posted_date")]
    public DateOnly? PostedDate { get; set; }

    [Column("is_recurring")]
    public bool IsRecurring { get; set; } = false;

    [Column("recurring_frequency")]
    [MaxLength(20)]
    public string? RecurringFrequency { get; set; } // DAILY, WEEKLY, MONTHLY, QUARTERLY, YEARLY

    [Column("is_categorized_manually")]
    public bool IsCategorizedManually { get; set; } = false;

    [Column("is_transfer")]
    public bool IsTransfer { get; set; } = false;

    [Column("transfer_to_account_id")]
    public Guid? TransferToAccountId { get; set; }

    [Column("external_transaction_id")]
    [MaxLength(255)]
    public string? ExternalTransactionId { get; set; }

    [Column("raw_data", TypeName = "jsonb")]
    public string? RawDataJson { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual LinkedAccount LinkedAccount { get; set; } = null!;
    public virtual LinkedAccount? TransferToAccount { get; set; }
    public virtual Merchant? Merchant { get; set; }
    public virtual Category? Category { get; set; }
    public virtual ICollection<TransactionFlag> Flags { get; set; } = new List<TransactionFlag>();
    public virtual ICollection<BudgetUtilization> BudgetUtilizations { get; set; } = new List<BudgetUtilization>();

    // Helper property for raw data
    [NotMapped]
    public JsonDocument? RawData
    {
        get => string.IsNullOrEmpty(RawDataJson) ? null : JsonDocument.Parse(RawDataJson);
        set => RawDataJson = value?.RootElement.GetRawText();
    }
}
