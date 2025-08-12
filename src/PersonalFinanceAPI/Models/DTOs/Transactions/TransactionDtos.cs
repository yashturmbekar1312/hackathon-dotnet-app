using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LinkedAccountId { get; set; }
    public string LinkedAccountName { get; set; } = string.Empty;
    public Guid? MerchantId { get; set; }
    public string? MerchantName { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "INR";
    public string TransactionType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ReferenceNumber { get; set; }
    public DateOnly TransactionDate { get; set; }
    public DateOnly? PostedDate { get; set; }
    public bool IsRecurring { get; set; }
    public string? RecurringFrequency { get; set; }
    public bool IsCategorizedManually { get; set; }
    public bool IsTransfer { get; set; }
    public Guid? TransferToAccountId { get; set; }
    public string? TransferToAccountName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTransactionRequest
{
    [Required]
    public Guid LinkedAccountId { get; set; }

    public Guid? MerchantId { get; set; }

    public Guid? CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public string CurrencyCode { get; set; } = "INR";

    [Required]
    public string TransactionType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ReferenceNumber { get; set; }

    [Required]
    public DateOnly TransactionDate { get; set; }

    public DateOnly? PostedDate { get; set; }

    public bool IsRecurring { get; set; } = false;

    public string? RecurringFrequency { get; set; }

    public bool IsTransfer { get; set; } = false;

    public Guid? TransferToAccountId { get; set; }

    public string? ExternalTransactionId { get; set; }
}

public class UpdateTransactionRequest
{
    public Guid? CategoryId { get; set; }
    public string? Description { get; set; }
    public bool? IsRecurring { get; set; }
    public string? RecurringFrequency { get; set; }
}

public class CategorizeTransactionRequest
{
    [Required]
    public Guid CategoryId { get; set; }
}

public class BulkCategorizeRequest
{
    [Required]
    public List<Guid> TransactionIds { get; set; } = new();

    [Required]
    public Guid CategoryId { get; set; }
}

public class TransactionQueryParameters
{
    public Guid? LinkedAccountId { get; set; }
    public Guid? CategoryId { get; set; }
    public string? TransactionType { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool? IsRecurring { get; set; }
    public bool? IsTransfer { get; set; }
    public bool? IsUncategorized { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "TransactionDate";
    public string SortOrder { get; set; } = "desc";
}

public class TransactionSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetSavings { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public List<CategorySpendingDto> TopExpenseCategories { get; set; } = new();
    public List<MerchantSpendingDto> TopMerchants { get; set; } = new();
}

public class CategorySpendingDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
}

public class MerchantSpendingDto
{
    public Guid MerchantId { get; set; }
    public string MerchantName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class ImportTransactionsRequest
{
    [Required]
    public Guid LinkedAccountId { get; set; }

    [Required]
    public IFormFile CsvFile { get; set; } = null!;

    public bool AutoCategorize { get; set; } = true;
}

public class SyncTransactionsRequest
{
    [Required]
    public Guid LinkedAccountId { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}
