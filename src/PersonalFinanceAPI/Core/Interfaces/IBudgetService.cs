using PersonalFinanceAPI.Models.Entities;
using PersonalFinanceAPI.Application.DTOs;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IBudgetService
{
    Task<Budget> CreateBudgetAsync(CreateBudgetRequest request, Guid userId);
    Task<Budget?> GetBudgetAsync(Guid budgetId, Guid userId);
    Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId);
    Task<Budget> UpdateBudgetAsync(Guid budgetId, UpdateBudgetRequest request, Guid userId);
    Task<bool> DeleteBudgetAsync(Guid budgetId, Guid userId);
    Task<BudgetUtilizationDto> GetBudgetUtilizationAsync(Guid budgetId, Guid userId);
    Task<IEnumerable<BudgetUtilizationDto>> GetAllBudgetUtilizationsAsync(Guid userId);
    Task<bool> IsOverBudgetAsync(Guid budgetId, Guid userId);
    Task<IEnumerable<Budget>> GetOverBudgetsAsync(Guid userId);
    Task<decimal> GetRemainingBudgetAsync(Guid budgetId, Guid userId);
}
