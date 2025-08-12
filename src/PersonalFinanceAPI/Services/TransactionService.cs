using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.DTOs.Transactions;
using PersonalFinanceAPI.Models.Entities;
using CsvHelper;
using System.Globalization;
using System.Text.Json;

namespace PersonalFinanceAPI.Services;

public interface ITransactionService
{
    Task<TransactionDto> CreateTransactionAsync(Guid userId, CreateTransactionRequest request);
    Task<TransactionDto> GetTransactionByIdAsync(Guid userId, Guid transactionId);
    Task<PagedResult<TransactionDto>> GetTransactionsAsync(Guid userId, TransactionQueryParameters parameters);
    Task<TransactionDto> UpdateTransactionAsync(Guid userId, Guid transactionId, UpdateTransactionRequest request);
    Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId);
    Task<TransactionDto> CategorizeTransactionAsync(Guid userId, Guid transactionId, CategorizeTransactionRequest request);
    Task<int> BulkCategorizeTransactionsAsync(Guid userId, BulkCategorizeRequest request);
    Task<List<TransactionDto>> GetUncategorizedTransactionsAsync(Guid userId);
    Task<List<TransactionDto>> GetRecurringTransactionsAsync(Guid userId);
    Task<TransactionSummaryDto> GetTransactionSummaryAsync(Guid userId, DateOnly? startDate = null, DateOnly? endDate = null);
    Task<List<TransactionDto>> ImportTransactionsFromCsvAsync(Guid userId, ImportTransactionsRequest request);
    Task<int> SyncTransactionsAsync(Guid userId, SyncTransactionsRequest request);
}

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(AppDbContext context, ILogger<TransactionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TransactionDto> CreateTransactionAsync(Guid userId, CreateTransactionRequest request)
    {
        _logger.LogInformation("Creating transaction for user {UserId}", userId);

        // Validate linked account belongs to user
        var linkedAccount = await _context.LinkedAccounts
            .FirstOrDefaultAsync(la => la.Id == request.LinkedAccountId && la.UserId == userId);

        if (linkedAccount == null)
        {
            throw new ArgumentException("Linked account not found or doesn't belong to user");
        }

        var transaction = new Transaction
        {
            UserId = userId,
            LinkedAccountId = request.LinkedAccountId,
            MerchantId = request.MerchantId,
            CategoryId = request.CategoryId,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            TransactionType = request.TransactionType.ToUpper(),
            Description = request.Description,
            ReferenceNumber = request.ReferenceNumber,
            TransactionDate = request.TransactionDate,
            PostedDate = request.PostedDate,
            IsRecurring = request.IsRecurring,
            RecurringFrequency = request.RecurringFrequency?.ToUpper(),
            IsTransfer = request.IsTransfer,
            TransferToAccountId = request.TransferToAccountId,
            ExternalTransactionId = request.ExternalTransactionId
        };

        // Auto-categorize if not provided
        if (transaction.CategoryId == null && transaction.MerchantId.HasValue)
        {
            var merchant = await _context.Merchants
                .FirstOrDefaultAsync(m => m.Id == transaction.MerchantId);

            if (merchant?.CategoryId != null)
            {
                transaction.CategoryId = merchant.CategoryId;
                _logger.LogInformation("Auto-categorized transaction {TransactionId} to category {CategoryId}", 
                    transaction.Id, transaction.CategoryId);
            }
        }

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        // Update budget utilizations if categorized
        if (transaction.CategoryId.HasValue)
        {
            await UpdateBudgetUtilizationsAsync(transaction);
        }

        _logger.LogInformation("Transaction {TransactionId} created successfully", transaction.Id);

        return await MapToTransactionDtoAsync(transaction);
    }

    public async Task<TransactionDto> GetTransactionByIdAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _context.Transactions
            .Include(t => t.LinkedAccount)
            .Include(t => t.Merchant)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        return await MapToTransactionDtoAsync(transaction);
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsAsync(Guid userId, TransactionQueryParameters parameters)
    {
        var query = _context.Transactions
            .Include(t => t.LinkedAccount)
            .Include(t => t.Merchant)
            .Include(t => t.Category)
            .Include(t => t.TransferToAccount)
            .Where(t => t.UserId == userId);

        // Apply filters
        if (parameters.LinkedAccountId.HasValue)
            query = query.Where(t => t.LinkedAccountId == parameters.LinkedAccountId);

        if (parameters.CategoryId.HasValue)
            query = query.Where(t => t.CategoryId == parameters.CategoryId);

        if (!string.IsNullOrEmpty(parameters.TransactionType))
            query = query.Where(t => t.TransactionType == parameters.TransactionType.ToUpper());

        if (parameters.StartDate.HasValue)
            query = query.Where(t => t.TransactionDate >= parameters.StartDate);

        if (parameters.EndDate.HasValue)
            query = query.Where(t => t.TransactionDate <= parameters.EndDate);

        if (parameters.MinAmount.HasValue)
            query = query.Where(t => t.Amount >= parameters.MinAmount);

        if (parameters.MaxAmount.HasValue)
            query = query.Where(t => t.Amount <= parameters.MaxAmount);

        if (parameters.IsRecurring.HasValue)
            query = query.Where(t => t.IsRecurring == parameters.IsRecurring);

        if (parameters.IsTransfer.HasValue)
            query = query.Where(t => t.IsTransfer == parameters.IsTransfer);

        if (parameters.IsUncategorized == true)
            query = query.Where(t => t.CategoryId == null);

        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(t => 
                (t.Description != null && t.Description.ToLower().Contains(searchTerm)) ||
                (t.Merchant != null && t.Merchant.Name.ToLower().Contains(searchTerm)) ||
                (t.ReferenceNumber != null && t.ReferenceNumber.ToLower().Contains(searchTerm)));
        }

        // Apply sorting
        query = parameters.SortBy.ToLower() switch
        {
            "amount" => parameters.SortOrder.ToLower() == "asc" 
                ? query.OrderBy(t => t.Amount) 
                : query.OrderByDescending(t => t.Amount),
            "merchant" => parameters.SortOrder.ToLower() == "asc" 
                ? query.OrderBy(t => t.Merchant != null ? t.Merchant.Name : "") 
                : query.OrderByDescending(t => t.Merchant != null ? t.Merchant.Name : ""),
            "category" => parameters.SortOrder.ToLower() == "asc" 
                ? query.OrderBy(t => t.Category != null ? t.Category.Name : "") 
                : query.OrderByDescending(t => t.Category != null ? t.Category.Name : ""),
            _ => parameters.SortOrder.ToLower() == "asc" 
                ? query.OrderBy(t => t.TransactionDate) 
                : query.OrderByDescending(t => t.TransactionDate)
        };

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)parameters.PageSize);

        var transactions = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        var transactionDtos = new List<TransactionDto>();
        foreach (var transaction in transactions)
        {
            transactionDtos.Add(await MapToTransactionDtoAsync(transaction));
        }

        return new PagedResult<TransactionDto>
        {
            Items = transactionDtos,
            TotalItems = totalItems,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalPages = totalPages
        };
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid userId, Guid transactionId, UpdateTransactionRequest request)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        // Update fields
        if (request.CategoryId.HasValue)
        {
            transaction.CategoryId = request.CategoryId;
            transaction.IsCategorizedManually = true;
        }

        if (!string.IsNullOrEmpty(request.Description))
            transaction.Description = request.Description;

        if (request.IsRecurring.HasValue)
            transaction.IsRecurring = request.IsRecurring.Value;

        if (!string.IsNullOrEmpty(request.RecurringFrequency))
            transaction.RecurringFrequency = request.RecurringFrequency.ToUpper();

        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Update budget utilizations if category changed
        if (request.CategoryId.HasValue)
        {
            await UpdateBudgetUtilizationsAsync(transaction);
        }

        _logger.LogInformation("Transaction {TransactionId} updated successfully", transactionId);

        return await MapToTransactionDtoAsync(transaction);
    }

    public async Task<bool> DeleteTransactionAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null)
        {
            return false;
        }

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Transaction {TransactionId} deleted successfully", transactionId);
        return true;
    }

    public async Task<TransactionDto> CategorizeTransactionAsync(Guid userId, Guid transactionId, CategorizeTransactionRequest request)
    {
        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.UserId == userId);

        if (transaction == null)
        {
            throw new ArgumentException("Transaction not found");
        }

        // Validate category exists
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId);

        if (category == null)
        {
            throw new ArgumentException("Category not found");
        }

        transaction.CategoryId = request.CategoryId;
        transaction.IsCategorizedManually = true;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Update budget utilizations
        await UpdateBudgetUtilizationsAsync(transaction);

        _logger.LogInformation("Transaction {TransactionId} categorized to {CategoryId}", transactionId, request.CategoryId);

        return await MapToTransactionDtoAsync(transaction);
    }

    public async Task<int> BulkCategorizeTransactionsAsync(Guid userId, BulkCategorizeRequest request)
    {
        var transactions = await _context.Transactions
            .Where(t => request.TransactionIds.Contains(t.Id) && t.UserId == userId)
            .ToListAsync();

        if (transactions.Count == 0)
        {
            return 0;
        }

        foreach (var transaction in transactions)
        {
            transaction.CategoryId = request.CategoryId;
            transaction.IsCategorizedManually = true;
            transaction.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // Update budget utilizations for all transactions
        foreach (var transaction in transactions)
        {
            await UpdateBudgetUtilizationsAsync(transaction);
        }

        _logger.LogInformation("Bulk categorized {Count} transactions to category {CategoryId}", 
            transactions.Count, request.CategoryId);

        return transactions.Count;
    }

    public async Task<List<TransactionDto>> GetUncategorizedTransactionsAsync(Guid userId)
    {
        var transactions = await _context.Transactions
            .Include(t => t.LinkedAccount)
            .Include(t => t.Merchant)
            .Where(t => t.UserId == userId && t.CategoryId == null)
            .OrderByDescending(t => t.TransactionDate)
            .Take(100) // Limit to 100 for performance
            .ToListAsync();

        var result = new List<TransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToTransactionDtoAsync(transaction));
        }

        return result;
    }

    public async Task<List<TransactionDto>> GetRecurringTransactionsAsync(Guid userId)
    {
        var transactions = await _context.Transactions
            .Include(t => t.LinkedAccount)
            .Include(t => t.Merchant)
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && t.IsRecurring)
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();

        var result = new List<TransactionDto>();
        foreach (var transaction in transactions)
        {
            result.Add(await MapToTransactionDtoAsync(transaction));
        }

        return result;
    }

    public async Task<TransactionSummaryDto> GetTransactionSummaryAsync(Guid userId, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        var query = _context.Transactions.Where(t => t.UserId == userId);

        if (startDate.HasValue)
            query = query.Where(t => t.TransactionDate >= startDate);

        if (endDate.HasValue)
            query = query.Where(t => t.TransactionDate <= endDate);

        var transactions = await query
            .Include(t => t.Category)
            .Include(t => t.Merchant)
            .ToListAsync();

        var totalIncome = transactions
            .Where(t => t.TransactionType == "CREDIT")
            .Sum(t => t.Amount);

        var totalExpenses = transactions
            .Where(t => t.TransactionType == "DEBIT")
            .Sum(t => t.Amount);

        var netSavings = totalIncome - totalExpenses;

        var categorySpending = transactions
            .Where(t => t.TransactionType == "DEBIT" && t.Category != null)
            .GroupBy(t => new { t.CategoryId, t.Category!.Name })
            .Select(g => new CategorySpendingDto
            {
                CategoryId = g.Key.CategoryId!.Value,
                CategoryName = g.Key.Name,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count(),
                Percentage = totalExpenses > 0 ? (g.Sum(t => t.Amount) / totalExpenses) * 100 : 0
            })
            .OrderByDescending(cs => cs.TotalAmount)
            .Take(10)
            .ToList();

        var merchantSpending = transactions
            .Where(t => t.TransactionType == "DEBIT" && t.Merchant != null)
            .GroupBy(t => new { t.MerchantId, t.Merchant!.Name })
            .Select(g => new MerchantSpendingDto
            {
                MerchantId = g.Key.MerchantId!.Value,
                MerchantName = g.Key.Name,
                TotalAmount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderByDescending(ms => ms.TotalAmount)
            .Take(10)
            .ToList();

        return new TransactionSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetSavings = netSavings,
            TransactionCount = transactions.Count,
            AverageTransactionAmount = transactions.Count > 0 ? transactions.Average(t => t.Amount) : 0,
            TopExpenseCategories = categorySpending,
            TopMerchants = merchantSpending
        };
    }

    public async Task<List<TransactionDto>> ImportTransactionsFromCsvAsync(Guid userId, ImportTransactionsRequest request)
    {
        _logger.LogInformation("Importing transactions from CSV for user {UserId}", userId);

        // Validate linked account
        var linkedAccount = await _context.LinkedAccounts
            .FirstOrDefaultAsync(la => la.Id == request.LinkedAccountId && la.UserId == userId);

        if (linkedAccount == null)
        {
            throw new ArgumentException("Linked account not found");
        }

        var importedTransactions = new List<Transaction>();

        using var reader = new StringReader(await ReadFormFileAsync(request.CsvFile));
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        try
        {
            var records = csv.GetRecords<CsvTransactionRecord>().ToList();

            foreach (var record in records)
            {
                // Skip if transaction with same external ID already exists
                if (!string.IsNullOrEmpty(record.ReferenceNumber))
                {
                    var existing = await _context.Transactions
                        .FirstOrDefaultAsync(t => t.ReferenceNumber == record.ReferenceNumber && 
                                                 t.LinkedAccountId == request.LinkedAccountId);
                    if (existing != null)
                    {
                        _logger.LogDebug("Skipping duplicate transaction with reference {Reference}", record.ReferenceNumber);
                        continue;
                    }
                }

                var transaction = new Transaction
                {
                    UserId = userId,
                    LinkedAccountId = request.LinkedAccountId,
                    Amount = Math.Abs(record.Amount),
                    TransactionType = record.Amount >= 0 ? "CREDIT" : "DEBIT",
                    Description = record.Description,
                    ReferenceNumber = record.ReferenceNumber,
                    TransactionDate = DateOnly.FromDateTime(record.Date),
                    PostedDate = DateOnly.FromDateTime(record.Date),
                    CurrencyCode = "INR"
                };

                // Auto-categorize if enabled
                if (request.AutoCategorize)
                {
                    await AutoCategorizeTransactionAsync(transaction);
                }

                importedTransactions.Add(transaction);
            }

            _context.Transactions.AddRange(importedTransactions);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Imported {Count} transactions from CSV", importedTransactions.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing transactions from CSV");
            throw new InvalidOperationException("Error parsing CSV file", ex);
        }

        var result = new List<TransactionDto>();
        foreach (var transaction in importedTransactions)
        {
            result.Add(await MapToTransactionDtoAsync(transaction));
        }

        return result;
    }

    public async Task<int> SyncTransactionsAsync(Guid userId, SyncTransactionsRequest request)
    {
        _logger.LogInformation("Syncing transactions for user {UserId} and account {AccountId}", userId, request.LinkedAccountId);

        // Validate linked account
        var linkedAccount = await _context.LinkedAccounts
            .FirstOrDefaultAsync(la => la.Id == request.LinkedAccountId && la.UserId == userId);

        if (linkedAccount == null)
        {
            throw new ArgumentException("Linked account not found");
        }

        // Simulate fetching transactions from bank aggregator
        var simulatedTransactions = GenerateSimulatedTransactions(request.LinkedAccountId, request.StartDate, request.EndDate);
        var syncedCount = 0;

        foreach (var simTransaction in simulatedTransactions)
        {
            // Check if transaction already exists
            var existing = await _context.Transactions
                .FirstOrDefaultAsync(t => t.ExternalTransactionId == simTransaction.ExternalTransactionId && 
                                         t.LinkedAccountId == request.LinkedAccountId);

            if (existing == null)
            {
                var transaction = new Transaction
                {
                    UserId = userId,
                    LinkedAccountId = request.LinkedAccountId,
                    Amount = simTransaction.Amount,
                    TransactionType = simTransaction.TransactionType,
                    Description = simTransaction.Description,
                    TransactionDate = simTransaction.TransactionDate,
                    PostedDate = simTransaction.PostedDate,
                    ExternalTransactionId = simTransaction.ExternalTransactionId,
                    CurrencyCode = "INR"
                };

                await AutoCategorizeTransactionAsync(transaction);
                _context.Transactions.Add(transaction);
                syncedCount++;
            }
        }

        if (syncedCount > 0)
        {
            linkedAccount.LastSyncedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("Synced {Count} new transactions", syncedCount);
        return syncedCount;
    }

    private async Task<TransactionDto> MapToTransactionDtoAsync(Transaction transaction)
    {
        // Load related entities if not already loaded
        if (transaction.LinkedAccount == null)
        {
            await _context.Entry(transaction)
                .Reference(t => t.LinkedAccount)
                .LoadAsync();
        }

        if (transaction.Merchant == null && transaction.MerchantId.HasValue)
        {
            await _context.Entry(transaction)
                .Reference(t => t.Merchant)
                .LoadAsync();
        }

        if (transaction.Category == null && transaction.CategoryId.HasValue)
        {
            await _context.Entry(transaction)
                .Reference(t => t.Category)
                .LoadAsync();
        }

        if (transaction.TransferToAccount == null && transaction.TransferToAccountId.HasValue)
        {
            await _context.Entry(transaction)
                .Reference(t => t.TransferToAccount)
                .LoadAsync();
        }

        return new TransactionDto
        {
            Id = transaction.Id,
            UserId = transaction.UserId,
            LinkedAccountId = transaction.LinkedAccountId,
            LinkedAccountName = transaction.LinkedAccount?.AccountName ?? "",
            MerchantId = transaction.MerchantId,
            MerchantName = transaction.Merchant?.Name,
            CategoryId = transaction.CategoryId,
            CategoryName = transaction.Category?.Name,
            Amount = transaction.Amount,
            CurrencyCode = transaction.CurrencyCode,
            TransactionType = transaction.TransactionType,
            Description = transaction.Description,
            ReferenceNumber = transaction.ReferenceNumber,
            TransactionDate = transaction.TransactionDate,
            PostedDate = transaction.PostedDate,
            IsRecurring = transaction.IsRecurring,
            RecurringFrequency = transaction.RecurringFrequency,
            IsCategorizedManually = transaction.IsCategorizedManually,
            IsTransfer = transaction.IsTransfer,
            TransferToAccountId = transaction.TransferToAccountId,
            TransferToAccountName = transaction.TransferToAccount?.AccountName,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };
    }

    private async Task UpdateBudgetUtilizationsAsync(Transaction transaction)
    {
        if (!transaction.CategoryId.HasValue || transaction.TransactionType != "DEBIT")
            return;

        var activeBudgets = await _context.Budgets
            .Where(b => b.UserId == transaction.UserId && 
                       b.CategoryId == transaction.CategoryId && 
                       b.IsActive &&
                       b.StartDate <= transaction.TransactionDate &&
                       b.EndDate >= transaction.TransactionDate)
            .ToListAsync();

        foreach (var budget in activeBudgets)
        {
            // Check if utilization already exists
            var existingUtilization = await _context.BudgetUtilizations
                .FirstOrDefaultAsync(bu => bu.BudgetId == budget.Id && bu.TransactionId == transaction.Id);

            if (existingUtilization == null)
            {
                var utilization = new BudgetUtilization
                {
                    BudgetId = budget.Id,
                    TransactionId = transaction.Id,
                    AmountUtilized = transaction.Amount,
                    UtilizationDate = transaction.TransactionDate
                };

                _context.BudgetUtilizations.Add(utilization);

                // Update budget current spent
                budget.CurrentSpent += transaction.Amount;
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task AutoCategorizeTransactionAsync(Transaction transaction)
    {
        if (!string.IsNullOrEmpty(transaction.Description))
        {
            var description = transaction.Description.ToLower();

            // Simple rule-based categorization
            var categoryKeywords = new Dictionary<string, string[]>
            {
                ["Food & Dining"] = new[] { "swiggy", "zomato", "restaurant", "food", "cafe", "pizza", "burger" },
                ["Transportation"] = new[] { "uber", "ola", "taxi", "metro", "bus", "petrol", "fuel", "parking" },
                ["Shopping"] = new[] { "amazon", "flipkart", "mall", "store", "shop", "purchase" },
                ["Entertainment"] = new[] { "netflix", "spotify", "movie", "cinema", "game", "entertainment" },
                ["Bills & Utilities"] = new[] { "electricity", "water", "gas", "internet", "mobile", "recharge", "bill" },
                ["Healthcare"] = new[] { "hospital", "doctor", "medicine", "pharmacy", "health", "medical" }
            };

            foreach (var kvp in categoryKeywords)
            {
                if (kvp.Value.Any(keyword => description.Contains(keyword)))
                {
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name == kvp.Key);

                    if (category != null)
                    {
                        transaction.CategoryId = category.Id;
                        _logger.LogDebug("Auto-categorized transaction to {CategoryName} based on description", kvp.Key);
                        break;
                    }
                }
            }
        }
    }

    private List<SimulatedTransaction> GenerateSimulatedTransactions(Guid linkedAccountId, DateOnly? startDate, DateOnly? endDate)
    {
        var start = startDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        var end = endDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        
        var transactions = new List<SimulatedTransaction>();
        var random = new Random();

        for (var date = start; date <= end; date = date.AddDays(1))
        {
            // Simulate 0-3 transactions per day
            var transactionCount = random.Next(0, 4);

            for (int i = 0; i < transactionCount; i++)
            {
                transactions.Add(new SimulatedTransaction
                {
                    ExternalTransactionId = Guid.NewGuid().ToString(),
                    Amount = random.Next(50, 2000),
                    TransactionType = random.NextDouble() > 0.8 ? "CREDIT" : "DEBIT",
                    Description = GetRandomDescription(random),
                    TransactionDate = date,
                    PostedDate = date
                });
            }
        }

        return transactions;
    }

    private string GetRandomDescription(Random random)
    {
        var descriptions = new[]
        {
            "Swiggy Order",
            "Uber Ride",
            "Amazon Purchase",
            "ATM Withdrawal",
            "Salary Credit",
            "Electricity Bill",
            "Mobile Recharge",
            "Netflix Subscription",
            "Coffee Shop",
            "Grocery Store",
            "Medical Expense",
            "Interest Credit"
        };

        return descriptions[random.Next(descriptions.Length)];
    }

    private async Task<string> ReadFormFileAsync(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        return await reader.ReadToEndAsync();
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CsvTransactionRecord
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}

public class SimulatedTransaction
{
    public string ExternalTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public DateOnly PostedDate { get; set; }
}
