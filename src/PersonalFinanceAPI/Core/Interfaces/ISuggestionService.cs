using PersonalFinanceAPI.Models.DTOs.Suggestions;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface ISuggestionService
{
    Task<IEnumerable<InvestmentSuggestionDto>> GetUserSuggestionsAsync(Guid userId, bool activeOnly = true);
    Task<InvestmentSuggestionDto?> GetSuggestionByIdAsync(Guid userId, Guid suggestionId);
    Task<InvestmentSuggestionDto> CreateSuggestionAsync(Guid userId, CreateSuggestionRequest request);
    Task<SuggestionActionResult> AcceptSuggestionAsync(Guid userId, Guid suggestionId, AcceptSuggestionRequest? request = null);
    Task<SuggestionActionResult> RejectSuggestionAsync(Guid userId, Guid suggestionId, RejectSuggestionRequest? request = null);
    Task<SuggestionActionResult> PostponeSuggestionAsync(Guid userId, Guid suggestionId, PostponeSuggestionRequest? request = null);
    Task<IEnumerable<SuggestionHistoryDto>> GetSuggestionHistoryAsync(Guid userId, Guid? suggestionId = null);
    Task GeneratePersonalizedSuggestionsAsync(Guid userId);
    Task<SuggestionStats> GetSuggestionStatsAsync(Guid userId);
}
