using PersonalFinanceAPI.Models.DTOs.Users;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfileAsync(Guid userId);
    Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request);
    Task<UserPreferencesDto> GetUserPreferencesAsync(Guid userId);
    Task<UserPreferencesDto> UpdateUserPreferencesAsync(Guid userId, UpdateUserPreferencesRequest request);
    Task<UserPreferencesDto> UpdateSavingsThresholdsAsync(Guid userId, UpdateSavingsThresholdsRequest request);
    Task<bool> DeleteUserAccountAsync(Guid userId);
}
