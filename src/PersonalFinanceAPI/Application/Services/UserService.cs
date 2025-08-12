using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Core.Exceptions;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.DTOs.Users;
using PersonalFinanceAPI.Models.Entities;

namespace PersonalFinanceAPI.Application.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth?.ToDateTime(TimeOnly.MinValue),
            Occupation = user.Occupation,
            Currency = user.Currency,
            AnnualIncome = user.AnnualIncome,
            IsEmailVerified = user.IsEmailVerified,
            IsPhoneVerified = user.IsPhoneVerified,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    public async Task<UserProfileDto> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;
        user.DateOfBirth = request.DateOfBirth.HasValue ? DateOnly.FromDateTime(request.DateOfBirth.Value) : null;
        user.Occupation = request.Occupation;
        user.Currency = request.Currency;
        user.AnnualIncome = request.AnnualIncome;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User profile updated for user {UserId}", userId);

        return await GetUserProfileAsync(userId);
    }

    public async Task<UserPreferencesDto> GetUserPreferencesAsync(Guid userId)
    {
        var preferences = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preferences == null)
        {
            // Create default preferences if they don't exist
            preferences = new UserPreferences
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ExpenseThreshold = 1000.00m,
                SavingsGoalMonthly = 5000.00m,
                InvestmentRiskLevel = "MODERATE",
                NotificationEmail = true,
                NotificationSms = false,
                NotificationPush = true,
                CurrencyCode = "INR",
                Timezone = "Asia/Kolkata",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserPreferences.Add(preferences);
            await _context.SaveChangesAsync();
        }

        return new UserPreferencesDto
        {
            Id = preferences.Id,
            UserId = preferences.UserId,
            ExpenseThreshold = preferences.ExpenseThreshold,
            SavingsGoalMonthly = preferences.SavingsGoalMonthly,
            InvestmentRiskLevel = preferences.InvestmentRiskLevel,
            NotificationEmail = preferences.NotificationEmail,
            NotificationSms = preferences.NotificationSms,
            NotificationPush = preferences.NotificationPush,
            CurrencyCode = preferences.CurrencyCode,
            Timezone = preferences.Timezone,
            CreatedAt = preferences.CreatedAt,
            UpdatedAt = preferences.UpdatedAt
        };
    }

    public async Task<UserPreferencesDto> UpdateUserPreferencesAsync(Guid userId, UpdateUserPreferencesRequest request)
    {
        var preferences = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preferences == null)
        {
            // Create new preferences if they don't exist
            await GetUserPreferencesAsync(userId);
            preferences = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // Update only provided fields
        if (request.ExpenseThreshold.HasValue)
            preferences!.ExpenseThreshold = request.ExpenseThreshold.Value;
        
        if (request.SavingsGoalMonthly.HasValue)
            preferences!.SavingsGoalMonthly = request.SavingsGoalMonthly.Value;
        
        if (!string.IsNullOrEmpty(request.InvestmentRiskLevel))
            preferences!.InvestmentRiskLevel = request.InvestmentRiskLevel;
        
        if (request.NotificationEmail.HasValue)
            preferences!.NotificationEmail = request.NotificationEmail.Value;
        
        if (request.NotificationSms.HasValue)
            preferences!.NotificationSms = request.NotificationSms.Value;
        
        if (request.NotificationPush.HasValue)
            preferences!.NotificationPush = request.NotificationPush.Value;
        
        if (!string.IsNullOrEmpty(request.CurrencyCode))
            preferences!.CurrencyCode = request.CurrencyCode;
        
        if (!string.IsNullOrEmpty(request.Timezone))
            preferences!.Timezone = request.Timezone;

        preferences!.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User preferences updated for user {UserId}", userId);

        return await GetUserPreferencesAsync(userId);
    }

    public async Task<UserPreferencesDto> UpdateSavingsThresholdsAsync(Guid userId, UpdateSavingsThresholdsRequest request)
    {
        var preferences = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (preferences == null)
        {
            // Create new preferences if they don't exist
            await GetUserPreferencesAsync(userId);
            preferences = await _context.UserPreferences
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        preferences!.SavingsGoalMonthly = request.SavingsGoalMonthly;
        
        if (request.ExpenseThreshold.HasValue)
            preferences.ExpenseThreshold = request.ExpenseThreshold.Value;

        preferences.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Savings thresholds updated for user {UserId}", userId);

        return await GetUserPreferencesAsync(userId);
    }

    public async Task<bool> DeleteUserAccountAsync(Guid userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

        if (user == null)
        {
            return false;
        }

        // Soft delete - mark as inactive
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        // Also revoke all active sessions
        var activeSessions = await _context.UserSessions
            .Where(s => s.UserId == userId && s.RevokedAt == null)
            .ToListAsync();

        foreach (var session in activeSessions)
        {
            session.RevokedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User account deleted (soft delete) for user {UserId}", userId);

        return true;
    }
}
