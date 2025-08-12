using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Core.Exceptions;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.DTOs.Accounts;
using PersonalFinanceAPI.Models.Entities;

namespace PersonalFinanceAPI.Application.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountService> _logger;

    public AccountService(AppDbContext context, ILogger<AccountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LinkedAccountDto> LinkAccountAsync(Guid userId, LinkAccountRequest request)
    {
        // Check if user exists
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId && u.IsActive);
        if (!userExists)
        {
            throw new NotFoundException("User not found");
        }

        // Check if account with same external ID already exists
        if (!string.IsNullOrEmpty(request.ExternalAccountId))
        {
            var existingAccount = await _context.LinkedAccounts
                .AnyAsync(a => a.UserId == userId && a.ExternalAccountId == request.ExternalAccountId);
            
            if (existingAccount)
            {
                throw new InvalidOperationException("Account with this external ID is already linked");
            }
        }

        var linkedAccount = new LinkedAccount
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BankAggregatorId = request.BankAggregatorId,
            AccountName = request.AccountName,
            AccountNumber = request.AccountNumber,
            AccountType = request.AccountType,
            BankName = request.BankName,
            Balance = request.Balance ?? 0m,
            CurrencyCode = request.CurrencyCode,
            IsActive = true,
            ExternalAccountId = request.ExternalAccountId,
            ConsentGivenAt = DateTime.UtcNow,
            ConsentExpiresAt = request.ConsentExpiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.LinkedAccounts.Add(linkedAccount);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Account linked successfully for user {UserId}, account {AccountId}", 
            userId, linkedAccount.Id);

        return await GetAccountByIdAsync(userId, linkedAccount.Id) ?? 
               throw new InvalidOperationException("Failed to retrieve linked account");
    }

    public async Task<IEnumerable<LinkedAccountDto>> GetUserAccountsAsync(Guid userId)
    {
        var accounts = await _context.LinkedAccounts
            .Include(a => a.BankAggregator)
            .Where(a => a.UserId == userId && a.IsActive)
            .OrderBy(a => a.AccountName)
            .ToListAsync();

        return accounts.Select(MapToDto);
    }

    public async Task<LinkedAccountDto?> GetAccountByIdAsync(Guid userId, Guid accountId)
    {
        var account = await _context.LinkedAccounts
            .Include(a => a.BankAggregator)
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId && a.IsActive);

        return account != null ? MapToDto(account) : null;
    }

    public async Task<LinkedAccountDto> UpdateAccountAsync(Guid userId, Guid accountId, UpdateAccountRequest request)
    {
        var account = await _context.LinkedAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId && a.IsActive);

        if (account == null)
        {
            throw new NotFoundException("Account not found");
        }

        if (!string.IsNullOrEmpty(request.AccountName))
            account.AccountName = request.AccountName;

        if (request.Balance.HasValue)
            account.Balance = request.Balance.Value;

        if (request.IsActive.HasValue)
            account.IsActive = request.IsActive.Value;

        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Account updated for user {UserId}, account {AccountId}", userId, accountId);

        return await GetAccountByIdAsync(userId, accountId) ?? 
               throw new InvalidOperationException("Failed to retrieve updated account");
    }

    public async Task<bool> UnlinkAccountAsync(Guid userId, Guid accountId)
    {
        var account = await _context.LinkedAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId && a.IsActive);

        if (account == null)
        {
            return false;
        }

        // Soft delete - mark as inactive
        account.IsActive = false;
        account.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Account unlinked for user {UserId}, account {AccountId}", userId, accountId);

        return true;
    }

    public async Task<bool> SyncAccountDataAsync(Guid userId, Guid accountId)
    {
        var account = await _context.LinkedAccounts
            .FirstOrDefaultAsync(a => a.Id == accountId && a.UserId == userId && a.IsActive);

        if (account == null)
        {
            return false;
        }

        // Simulate data sync process
        // In a real implementation, this would call external banking APIs
        
        account.LastSyncedAt = DateTime.UtcNow;
        account.UpdatedAt = DateTime.UtcNow;

        // Generate some sample transactions for demonstration
        await GenerateSampleTransactionsAsync(userId, accountId);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Account data synced for user {UserId}, account {AccountId}", userId, accountId);

        return true;
    }

    public async Task<IEnumerable<LinkedAccountDto>> SyncAllAccountsAsync(Guid userId)
    {
        var accounts = await _context.LinkedAccounts
            .Where(a => a.UserId == userId && a.IsActive)
            .ToListAsync();

        foreach (var account in accounts)
        {
            await SyncAccountDataAsync(userId, account.Id);
        }

        return await GetUserAccountsAsync(userId);
    }

    public async Task<IEnumerable<BankAggregatorDto>> GetBankAggregatorsAsync()
    {
        var aggregators = await _context.BankAggregators
            .Where(ba => ba.IsActive)
            .OrderBy(ba => ba.Name)
            .ToListAsync();

        return aggregators.Select(ba => new BankAggregatorDto
        {
            Id = ba.Id,
            Name = ba.Name,
            ApiEndpoint = ba.ApiEndpoint,
            IsActive = ba.IsActive,
            CreatedAt = ba.CreatedAt,
            UpdatedAt = ba.UpdatedAt
        });
    }

    private static LinkedAccountDto MapToDto(LinkedAccount account)
    {
        return new LinkedAccountDto
        {
            Id = account.Id,
            UserId = account.UserId,
            BankAggregatorId = account.BankAggregatorId,
            AccountName = account.AccountName,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            BankName = account.BankName,
            Balance = account.Balance,
            CurrencyCode = account.CurrencyCode,
            IsActive = account.IsActive,
            LastSyncedAt = account.LastSyncedAt,
            ExternalAccountId = account.ExternalAccountId,
            ConsentGivenAt = account.ConsentGivenAt,
            ConsentExpiresAt = account.ConsentExpiresAt,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt,
            BankAggregatorName = account.BankAggregator?.Name
        };
    }

    private async Task GenerateSampleTransactionsAsync(Guid userId, Guid accountId)
    {
        // Get some sample categories and merchants
        var foodCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Food & Dining");
        var transportCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Transportation");
        var salaryCategory = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == "Salary");

        var swiggyMerchant = await _context.Merchants
            .FirstOrDefaultAsync(m => m.Name == "Swiggy");
        var uberMerchant = await _context.Merchants
            .FirstOrDefaultAsync(m => m.Name == "Uber");

        var random = new Random();
        var baseDate = DateTime.UtcNow.Date.AddDays(-30);

        var sampleTransactions = new List<Transaction>();

        // Generate some sample transactions for the last 30 days
        for (int i = 0; i < 15; i++)
        {
            var transactionDate = baseDate.AddDays(random.Next(0, 30));
            
            if (i % 5 == 0) // Every 5th transaction is income
            {
                sampleTransactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LinkedAccountId = accountId,
                    CategoryId = salaryCategory?.Id,
                    Amount = random.Next(25000, 75000),
                    CurrencyCode = "INR",
                    TransactionType = "CREDIT",
                    Description = "Salary Credit",
                    ReferenceNumber = $"SAL{random.Next(100000, 999999)}",
                    TransactionDate = DateOnly.FromDateTime(transactionDate),
                    PostedDate = DateOnly.FromDateTime(transactionDate),
                    IsRecurring = i == 0, // First salary transaction is recurring
                    RecurringFrequency = i == 0 ? "MONTHLY" : null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            else
            {
                var isFood = random.Next(0, 2) == 0;
                sampleTransactions.Add(new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    LinkedAccountId = accountId,
                    MerchantId = isFood ? swiggyMerchant?.Id : uberMerchant?.Id,
                    CategoryId = isFood ? foodCategory?.Id : transportCategory?.Id,
                    Amount = random.Next(50, 1500),
                    CurrencyCode = "INR",
                    TransactionType = "DEBIT",
                    Description = isFood ? "Food order" : "Ride booking",
                    ReferenceNumber = $"TXN{random.Next(100000, 999999)}",
                    TransactionDate = DateOnly.FromDateTime(transactionDate),
                    PostedDate = DateOnly.FromDateTime(transactionDate),
                    IsRecurring = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        _context.Transactions.AddRange(sampleTransactions);
    }
}
