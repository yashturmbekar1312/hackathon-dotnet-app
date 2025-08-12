using PersonalFinanceAPI.Models.DTOs.Alerts;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IAlertService
{
    Task<IEnumerable<UserAlertDto>> GetUserAlertsAsync(Guid userId, bool unreadOnly = false);
    Task<UserAlertDto?> GetAlertByIdAsync(Guid userId, Guid alertId);
    Task<UserAlertDto> CreateAlertAsync(Guid userId, CreateAlertRequest request);
    Task<bool> AcknowledgeAlertAsync(Guid userId, Guid alertId);
    Task<bool> MarkAlertAsReadAsync(Guid userId, Guid alertId);
    Task<bool> DeleteAlertAsync(Guid userId, Guid alertId);
    Task<AlertPreferencesDto> GetAlertPreferencesAsync(Guid userId);
    Task<AlertPreferencesDto> UpdateAlertPreferencesAsync(Guid userId, UpdateAlertPreferencesRequest request);
    Task<int> GetUnreadAlertCountAsync(Guid userId);
    Task GenerateBudgetBreachAlertAsync(Guid userId, Guid budgetId, decimal amount);
    Task GenerateExpenseThresholdAlertAsync(Guid userId, decimal amount);
    Task GenerateLowSavingsAlertAsync(Guid userId, decimal currentSavings);
}
