using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Models.Entities;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Models.Enums;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Application.DTOs;

namespace PersonalFinanceAPI.Application.Services;

public class BudgetService : IBudgetService
{
    private readonly AppDbContext _context;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(AppDbContext context, ILogger<BudgetService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Budget> CreateBudgetAsync(CreateBudgetRequest request, Guid userId)
    {
        _logger.LogInformation("Creating budget for user {UserId}", userId);

        // Validate category
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId);
        
        if (!categoryExists)
        {
            throw new ArgumentException("Category not found");
        }

        // Set default dates based on period type if not provided
        var startDateTime = GetPeriodStart(request.PeriodType);
        var endDateTime = GetPeriodEnd(request.PeriodType, startDateTime);
        
        var startDate = request.StartDate ?? DateOnly.FromDateTime(startDateTime);
        var endDate = request.EndDate ?? DateOnly.FromDateTime(endDateTime);

        var budget = new Budget
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CategoryId = request.CategoryId,
            BudgetAmount = request.BudgetAmount,
            PeriodType = request.PeriodType.ToString(),
            StartDate = startDate,
            EndDate = endDate,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Budget {BudgetId} created successfully for user {UserId}", budget.Id, userId);
        return budget;
    }

    public async Task<Budget?> GetBudgetAsync(Guid budgetId, Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == budgetId && b.UserId == userId);
    }

    public async Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId)
    {
        return await _context.Budgets
            .Include(b => b.Category)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Budget> UpdateBudgetAsync(Guid budgetId, UpdateBudgetRequest request, Guid userId)
    {
        var budget = await GetBudgetAsync(budgetId, userId);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        // Validate category if being updated
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId.Value);
            
            if (!categoryExists)
            {
                throw new ArgumentException("Category not found");
            }
            budget.CategoryId = request.CategoryId.Value;
        }

        // Update fields if provided
        if (request.BudgetAmount.HasValue)
            budget.BudgetAmount = request.BudgetAmount.Value;
        
        if (request.PeriodType.HasValue)
            budget.PeriodType = request.PeriodType.Value.ToString();
        
        if (request.StartDate.HasValue)
            budget.StartDate = request.StartDate.Value;
        
        if (request.EndDate.HasValue)
            budget.EndDate = request.EndDate.Value;
        
        if (request.IsActive.HasValue)
            budget.IsActive = request.IsActive.Value;

        budget.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return budget;
    }

    public async Task<bool> DeleteBudgetAsync(Guid budgetId, Guid userId)
    {
        var budget = await GetBudgetAsync(budgetId, userId);
        if (budget == null)
        {
            return false;
        }

        _context.Budgets.Remove(budget);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BudgetUtilizationDto> GetBudgetUtilizationAsync(Guid budgetId, Guid userId)
    {
        var budget = await GetBudgetAsync(budgetId, userId);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        // Update current spent amount
        await UpdateBudgetSpentAsync(budget);

        return new BudgetUtilizationDto
        {
            BudgetId = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = budget.Category?.Name ?? "Unknown",
            BudgetAmount = budget.BudgetAmount,
            CurrentSpent = budget.CurrentSpent,
            RemainingAmount = budget.RemainingAmount,
            UtilizationPercentage = budget.UtilizationPercentage,
            IsOverBudget = budget.IsOverBudget,
            PeriodType = budget.PeriodType,
            StartDate = budget.StartDate,
            EndDate = budget.EndDate
        };
    }

    public async Task<IEnumerable<BudgetUtilizationDto>> GetAllBudgetUtilizationsAsync(Guid userId)
    {
        var budgets = await GetUserBudgetsAsync(userId);
        var utilizations = new List<BudgetUtilizationDto>();

        foreach (var budget in budgets)
        {
            await UpdateBudgetSpentAsync(budget);
            utilizations.Add(new BudgetUtilizationDto
            {
                BudgetId = budget.Id,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate
            });
        }

        return utilizations;
    }

    public async Task<bool> IsOverBudgetAsync(Guid budgetId, Guid userId)
    {
        var budget = await GetBudgetAsync(budgetId, userId);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        await UpdateBudgetSpentAsync(budget);
        return budget.IsOverBudget;
    }

    public async Task<IEnumerable<Budget>> GetOverBudgetsAsync(Guid userId)
    {
        var budgets = await GetUserBudgetsAsync(userId);
        var overBudgets = new List<Budget>();

        foreach (var budget in budgets)
        {
            await UpdateBudgetSpentAsync(budget);
            if (budget.IsOverBudget)
            {
                overBudgets.Add(budget);
            }
        }

        return overBudgets;
    }

    public async Task<decimal> GetRemainingBudgetAsync(Guid budgetId, Guid userId)
    {
        var budget = await GetBudgetAsync(budgetId, userId);
        if (budget == null)
        {
            throw new ArgumentException("Budget not found");
        }

        await UpdateBudgetSpentAsync(budget);
        return budget.RemainingAmount;
    }

    private async Task UpdateBudgetSpentAsync(Budget budget)
    {
        var spentAmount = await _context.Transactions
            .Where(t => t.UserId == budget.UserId && 
                       t.CategoryId == budget.CategoryId &&
                       t.TransactionDate >= budget.StartDate &&
                       t.TransactionDate <= budget.EndDate &&
                       t.Amount < 0) // Negative amounts represent expenses
            .SumAsync(t => Math.Abs(t.Amount));

        budget.CurrentSpent = spentAmount;
        budget.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
    }

    private DateTime GetPeriodStart(PeriodType periodType)
    {
        var now = DateTime.UtcNow;
        return periodType switch
        {
            PeriodType.WEEKLY => now.StartOfWeek(),
            PeriodType.MONTHLY => new DateTime(now.Year, now.Month, 1),
            PeriodType.QUARTERLY => new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1),
            PeriodType.YEARLY => new DateTime(now.Year, 1, 1),
            _ => now.Date
        };
    }

    private DateTime GetPeriodEnd(PeriodType periodType, DateTime startDate)
    {
        return periodType switch
        {
            PeriodType.WEEKLY => startDate.AddDays(7).AddTicks(-1),
            PeriodType.MONTHLY => startDate.AddMonths(1).AddTicks(-1),
            PeriodType.QUARTERLY => startDate.AddMonths(3).AddTicks(-1),
            PeriodType.YEARLY => startDate.AddYears(1).AddTicks(-1),
            _ => startDate.AddDays(1).AddTicks(-1)
        };
    }
}

public static class DateTimeExtensions
{
    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
    {
        int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
        return dt.AddDays(-1 * diff).Date;
    }
}
