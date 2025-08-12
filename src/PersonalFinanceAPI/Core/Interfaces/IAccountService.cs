using PersonalFinanceAPI.Models.DTOs.Accounts;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IAccountService
{
    Task<LinkedAccountDto> LinkAccountAsync(Guid userId, LinkAccountRequest request);
    Task<IEnumerable<LinkedAccountDto>> GetUserAccountsAsync(Guid userId);
    Task<LinkedAccountDto?> GetAccountByIdAsync(Guid userId, Guid accountId);
    Task<LinkedAccountDto> UpdateAccountAsync(Guid userId, Guid accountId, UpdateAccountRequest request);
    Task<bool> UnlinkAccountAsync(Guid userId, Guid accountId);
    Task<bool> SyncAccountDataAsync(Guid userId, Guid accountId);
    Task<IEnumerable<LinkedAccountDto>> SyncAllAccountsAsync(Guid userId);
    Task<IEnumerable<BankAggregatorDto>> GetBankAggregatorsAsync();
}
