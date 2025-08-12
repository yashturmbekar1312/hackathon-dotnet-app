using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Accounts;

public class LinkedAccountDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? BankAggregatorId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? LastSyncedAt { get; set; }
    public string? ExternalAccountId { get; set; }
    public DateTime ConsentGivenAt { get; set; }
    public DateTime? ConsentExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? BankAggregatorName { get; set; }
}

public class LinkAccountRequest
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string AccountName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [Required]
    [RegularExpression("^(SAVINGS|CURRENT|CREDIT_CARD|INVESTMENT)$")]
    public string AccountType { get; set; } = string.Empty;

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string BankName { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal? Balance { get; set; }

    [StringLength(3, MinimumLength = 3)]
    [RegularExpression("^[A-Z]{3}$")]
    public string CurrencyCode { get; set; } = "INR";

    public Guid? BankAggregatorId { get; set; }

    [StringLength(255)]
    public string? ExternalAccountId { get; set; }

    public DateTime? ConsentExpiresAt { get; set; }
}

public class UpdateAccountRequest
{
    [StringLength(255, MinimumLength = 1)]
    public string? AccountName { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Balance { get; set; }

    public bool? IsActive { get; set; }
}

public class SyncAccountRequest
{
    public bool ForceSync { get; set; } = false;
    public DateTime? SyncFromDate { get; set; }
}

public class BankAggregatorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ApiEndpoint { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
