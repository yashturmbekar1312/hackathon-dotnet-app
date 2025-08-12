using PersonalFinanceAPI.Models.DTOs.Goals;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IGoalService
{
    Task<IEnumerable<FinancialGoalDto>> GetUserGoalsAsync(Guid userId, bool activeOnly = true);
    Task<FinancialGoalDto?> GetGoalByIdAsync(Guid userId, Guid goalId);
    Task<FinancialGoalDto> CreateGoalAsync(Guid userId, CreateGoalRequest request);
    Task<FinancialGoalDto> UpdateGoalAsync(Guid userId, Guid goalId, UpdateGoalRequest request);
    Task<bool> DeleteGoalAsync(Guid userId, Guid goalId);
    Task<GoalContributionResult> AddContributionAsync(Guid userId, Guid goalId, ContributeToGoalRequest request);
    Task<IEnumerable<GoalProgressDto>> GetGoalProgressAsync(Guid userId);
    Task<GoalInsightsDto> GetGoalInsightsAsync(Guid userId, Guid goalId);
}
