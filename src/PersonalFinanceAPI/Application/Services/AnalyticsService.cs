using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.DTOs.Analytics;
using PersonalFinanceAPI.Models.Entities;

namespace PersonalFinanceAPI.Application.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(AppDbContext context, ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardDto> GetDashboardAsync(Guid userId)
    {
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startOfMonth = currentMonth;
        var endOfMonth = currentMonth.AddMonths(1).AddDays(-1);

        // Get total balance across all accounts
        var totalBalance = await _context.LinkedAccounts
            .Where(a => a.UserId == userId && a.IsActive)
            .SumAsync(a => a.Balance);

        // Get monthly transactions
        var monthlyTransactions = await _context.Transactions
            .Where(t => t.UserId == userId && 
                       t.TransactionDate >= DateOnly.FromDateTime(startOfMonth) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endOfMonth))
            .ToListAsync();

        var monthlyIncome = monthlyTransactions
            .Where(t => t.TransactionType == "CREDIT")
            .Sum(t => t.Amount);

        var monthlyExpenses = monthlyTransactions
            .Where(t => t.TransactionType == "DEBIT")
            .Sum(t => t.Amount);

        var monthlySavings = monthlyIncome - monthlyExpenses;
        var savingsRate = monthlyIncome > 0 ? (monthlySavings / monthlyIncome) * 100 : 0;

        // Get total transaction count
        var totalTransactions = await _context.Transactions
            .CountAsync(t => t.UserId == userId);

        // Get unread alerts count
        var unreadAlerts = await _context.UserAlerts
            .CountAsync(a => a.UserId == userId && !a.IsRead);

        // Get active goals count
        var activeGoals = await _context.FinancialGoals
            .CountAsync(g => g.UserId == userId && g.IsActive);

        // Get pending suggestions count
        var pendingSuggestions = await _context.InvestmentSuggestions
            .CountAsync(s => s.UserId == userId && s.IsActive);

        // Get top expense categories
        var topExpenseCategories = await GetCategorySpendingAsync(userId, startOfMonth, endOfMonth);

        // Get recent transactions
        var recentTransactions = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.Merchant)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(10)
            .Select(t => new TransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                TransactionType = t.TransactionType,
                Description = t.Description ?? string.Empty,
                TransactionDate = t.TransactionDate.ToDateTime(TimeOnly.MinValue),
                CategoryName = t.Category != null ? t.Category.Name : null,
                MerchantName = t.Merchant != null ? t.Merchant.Name : null
            })
            .ToListAsync();

        // Get budget overview
        var budgetOverview = await GetBudgetOverviewAsync(userId);

        return new DashboardDto
        {
            UserId = userId,
            TotalBalance = totalBalance,
            MonthlyIncome = monthlyIncome,
            MonthlyExpenses = monthlyExpenses,
            MonthlySavings = monthlySavings,
            SavingsRate = savingsRate,
            TotalTransactions = totalTransactions,
            UnreadAlerts = unreadAlerts,
            ActiveGoals = activeGoals,
            PendingSuggestions = pendingSuggestions,
            TopExpenseCategories = topExpenseCategories.Take(5),
            RecentTransactions = recentTransactions,
            BudgetOverview = budgetOverview,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<MonthlyReportDto> GetMonthlyReportAsync(Guid userId, int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && 
                       t.TransactionDate >= DateOnly.FromDateTime(startDate) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endDate))
            .ToListAsync();

        var totalIncome = transactions
            .Where(t => t.TransactionType == "CREDIT")
            .Sum(t => t.Amount);

        var totalExpenses = transactions
            .Where(t => t.TransactionType == "DEBIT")
            .Sum(t => t.Amount);

        var netSavings = totalIncome - totalExpenses;
        var transactionCount = transactions.Count;
        var averageTransaction = transactionCount > 0 ? transactions.Average(t => t.Amount) : 0;
        var savingsRate = totalIncome > 0 ? (netSavings / totalIncome) * 100 : 0;

        var topExpenseCategory = transactions
            .Where(t => t.TransactionType == "DEBIT" && t.Category != null)
            .GroupBy(t => t.Category!.Name)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .FirstOrDefault();

        var categoryBreakdown = await GetCategorySpendingAsync(userId, startDate, endDate);
        var dailySpending = GetDailySpending(transactions, startDate, endDate);

        return new MonthlyReportDto
        {
            UserId = userId,
            MonthYear = startDate,
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            NetSavings = netSavings,
            TopExpenseCategory = topExpenseCategory?.Key,
            TopExpenseAmount = topExpenseCategory?.Sum(t => t.Amount) ?? 0,
            TransactionCount = transactionCount,
            AverageTransactionAmount = averageTransaction,
            SavingsRate = savingsRate,
            IsFinal = DateTime.UtcNow > endDate,
            ComputedAt = DateTime.UtcNow,
            CategoryBreakdown = categoryBreakdown,
            DailySpending = dailySpending
        };
    }

    public async Task<IEnumerable<MonthlyReportDto>> GetMonthlyReportsAsync(Guid userId, int? year = null)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;
        var reports = new List<MonthlyReportDto>();

        for (int month = 1; month <= 12; month++)
        {
            var report = await GetMonthlyReportAsync(userId, targetYear, month);
            if (report.TransactionCount > 0 || DateTime.UtcNow >= new DateTime(targetYear, month, 1))
            {
                reports.Add(report);
            }
        }

        return reports;
    }

    public async Task<SavingsSummaryDto> GetSavingsSummaryAsync(Guid userId)
    {
        var preferences = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var savingsGoal = preferences?.SavingsGoalMonthly ?? 5000m;

        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var currentMonthSavings = await CalculateSavingsForMonth(userId, currentMonth);

        var lastSixMonths = new List<MonthlySavingsDto>();
        for (int i = 5; i >= 0; i--)
        {
            var month = currentMonth.AddMonths(-i);
            var savings = await CalculateSavingsForMonth(userId, month);
            lastSixMonths.Add(new MonthlySavingsDto
            {
                Month = month,
                Amount = savings,
                Goal = savingsGoal,
                Achievement = savingsGoal > 0 ? (savings / savingsGoal) * 100 : 0
            });
        }

        var averageMonthlySavings = lastSixMonths.Where(s => s.Amount != 0).DefaultIfEmpty().Average(s => s?.Amount ?? 0);
        var totalSavings = lastSixMonths.Sum(s => s.Amount);
        var projectedAnnualSavings = averageMonthlySavings * 12;
        var savingsProgress = savingsGoal > 0 ? (currentMonthSavings / savingsGoal) * 100 : 0;
        var onTrack = currentMonthSavings >= savingsGoal;
        var deficitOrSurplus = currentMonthSavings - savingsGoal;

        return new SavingsSummaryDto
        {
            UserId = userId,
            CurrentMonthSavings = currentMonthSavings,
            SavingsGoal = savingsGoal,
            SavingsProgress = savingsProgress,
            AverageMonthlySavings = averageMonthlySavings,
            TotalSavings = totalSavings,
            ProjectedAnnualSavings = projectedAnnualSavings,
            OnTrackToMeetGoal = onTrack,
            DeficitOrSurplus = deficitOrSurplus,
            LastSixMonths = lastSixMonths
        };
    }

    public async Task<SavingsProjectionDto> GetSavingsProjectionAsync(Guid userId, int months = 12)
    {
        var preferences = await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var monthlyGoal = preferences?.SavingsGoalMonthly ?? 5000m;
        var currentSavings = await GetCurrentSavingsAsync(userId);

        var projections = new List<ProjectionDto>();
        var cumulativeSavings = currentSavings;

        for (int i = 1; i <= months; i++)
        {
            var month = DateTime.UtcNow.AddMonths(i);
            cumulativeSavings += monthlyGoal;

            projections.Add(new ProjectionDto
            {
                Month = month,
                ProjectedSavings = monthlyGoal,
                CumulativeSavings = cumulativeSavings
            });
        }

        return new SavingsProjectionDto
        {
            UserId = userId,
            CurrentSavings = currentSavings,
            MonthlyGoal = monthlyGoal,
            Projections = projections,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<CategorySpendingDto>> GetCategorySpendingAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = toDate ?? DateTime.UtcNow;

        var categorySpending = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && 
                       t.TransactionType == "DEBIT" &&
                       t.TransactionDate >= DateOnly.FromDateTime(startDate) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endDate))
            .GroupBy(t => new { t.CategoryId, t.Category!.Name, t.Category.Color, t.Category.Icon })
            .Select(g => new CategorySpendingDto
            {
                CategoryId = g.Key.CategoryId ?? Guid.Empty,
                CategoryName = g.Key.Name ?? "Uncategorized",
                Amount = g.Sum(t => t.Amount),
                TransactionCount = g.Count(),
                Color = g.Key.Color,
                Icon = g.Key.Icon,
                Percentage = 0 // Will be calculated after getting total
            })
            .OrderByDescending(c => c.Amount)
            .ToListAsync();

        var totalSpent = categorySpending.Sum(c => c.Amount);
        if (totalSpent > 0)
        {
            foreach (var category in categorySpending)
            {
                category.Percentage = (category.Amount / totalSpent) * 100;
            }
        }

        return categorySpending;
    }

    public async Task<IEnumerable<TransactionTrendDto>> GetTransactionTrendsAsync(Guid userId, string period = "monthly")
    {
        // Implementation for transaction trends would depend on the period
        // For now, implementing monthly trends for the last 12 months
        var trends = new List<TransactionTrendDto>();
        var startDate = DateTime.UtcNow.AddMonths(-11);

        for (int i = 0; i < 12; i++)
        {
            var month = startDate.AddMonths(i);
            var monthStart = new DateTime(month.Year, month.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && 
                           t.TransactionDate >= DateOnly.FromDateTime(monthStart) && 
                           t.TransactionDate <= DateOnly.FromDateTime(monthEnd))
                .ToListAsync();

            var income = transactions.Where(t => t.TransactionType == "CREDIT").Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.TransactionType == "DEBIT").Sum(t => t.Amount);

            trends.Add(new TransactionTrendDto
            {
                Period = monthStart,
                Income = income,
                Expenses = expenses,
                NetAmount = income - expenses,
                TransactionCount = transactions.Count
            });
        }

        return trends;
    }

    public async Task<ExpenseAnalysisDto> GetExpenseAnalysisAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = toDate ?? DateTime.UtcNow;
        var daysDiff = (endDate - startDate).Days;

        var expenses = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && 
                       t.TransactionType == "DEBIT" &&
                       t.TransactionDate >= DateOnly.FromDateTime(startDate) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endDate))
            .ToListAsync();

        var totalExpenses = expenses.Sum(t => t.Amount);
        var averageDaily = daysDiff > 0 ? totalExpenses / daysDiff : 0;
        var averageWeekly = averageDaily * 7;
        var averageMonthly = averageDaily * 30;

        var topCategory = expenses
            .Where(t => t.Category != null)
            .GroupBy(t => t.Category!.Name)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .FirstOrDefault();

        var categoryBreakdown = await GetCategorySpendingAsync(userId, startDate, endDate);

        var insights = GenerateSpendingInsights(expenses, totalExpenses, averageDaily);

        return new ExpenseAnalysisDto
        {
            UserId = userId,
            FromDate = startDate,
            ToDate = endDate,
            TotalExpenses = totalExpenses,
            AverageDaily = averageDaily,
            AverageWeekly = averageWeekly,
            AverageMonthly = averageMonthly,
            TopExpenseCategory = topCategory?.Key ?? "None",
            TopExpenseAmount = topCategory?.Sum(t => t.Amount) ?? 0,
            CategoryBreakdown = categoryBreakdown,
            SpendingInsights = insights
        };
    }

    public async Task ComputeMonthlySummaryAsync(Guid userId, DateTime monthYear)
    {
        var startDate = new DateTime(monthYear.Year, monthYear.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.UserId == userId && 
                       t.TransactionDate >= DateOnly.FromDateTime(startDate) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endDate))
            .ToListAsync();

        var totalIncome = transactions.Where(t => t.TransactionType == "CREDIT").Sum(t => t.Amount);
        var totalExpenses = transactions.Where(t => t.TransactionType == "DEBIT").Sum(t => t.Amount);
        var netSavings = totalIncome - totalExpenses;

        var topExpenseCategory = transactions
            .Where(t => t.TransactionType == "DEBIT" && t.Category != null)
            .GroupBy(t => t.Category!)
            .OrderByDescending(g => g.Sum(t => t.Amount))
            .FirstOrDefault();

        var existingSummary = await _context.MonthlySummaries
            .FirstOrDefaultAsync(ms => ms.UserId == userId && 
                                      ms.MonthYear == DateOnly.FromDateTime(startDate));

        if (existingSummary != null)
        {
            existingSummary.TotalIncome = totalIncome;
            existingSummary.TotalExpenses = totalExpenses;
            existingSummary.NetSavings = netSavings;
            existingSummary.TopExpenseCategoryId = topExpenseCategory?.Key.Id;
            existingSummary.TopExpenseAmount = topExpenseCategory?.Sum(t => t.Amount) ?? 0;
            existingSummary.TransactionCount = transactions.Count;
            existingSummary.AverageTransactionAmount = transactions.Count > 0 ? transactions.Average(t => t.Amount) : 0;
            existingSummary.SavingsRate = totalIncome > 0 ? (netSavings / totalIncome) * 100 : 0;
            existingSummary.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            var newSummary = new MonthlySummary
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MonthYear = DateOnly.FromDateTime(startDate),
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetSavings = netSavings,
                TopExpenseCategoryId = topExpenseCategory?.Key.Id,
                TopExpenseAmount = topExpenseCategory?.Sum(t => t.Amount) ?? 0,
                TransactionCount = transactions.Count,
                AverageTransactionAmount = transactions.Count > 0 ? transactions.Average(t => t.Amount) : 0,
                SavingsRate = totalIncome > 0 ? (netSavings / totalIncome) * 100 : 0,
                IsFinal = DateTime.UtcNow > endDate,
                ComputedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MonthlySummaries.Add(newSummary);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RecalculateUserDataAsync(Guid userId)
    {
        // Recalculate monthly summaries for the last 12 months
        var startDate = DateTime.UtcNow.AddMonths(-12);
        
        for (int i = 0; i < 13; i++)
        {
            var month = startDate.AddMonths(i);
            await ComputeMonthlySummaryAsync(userId, month);
        }

        _logger.LogInformation("User data recalculated for user {UserId}", userId);
    }

    private async Task<BudgetOverviewDto> GetBudgetOverviewAsync(Guid userId)
    {
        var currentMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var budgets = await _context.Budgets
            .Where(b => b.UserId == userId && b.IsActive &&
                       b.StartDate <= DateOnly.FromDateTime(currentMonth) &&
                       b.EndDate >= DateOnly.FromDateTime(currentMonth))
            .ToListAsync();

        var totalBudgets = budgets.Count;
        var totalBudgetAmount = budgets.Sum(b => b.BudgetAmount);
        var totalSpent = budgets.Sum(b => b.CurrentSpent);
        
        var budgetsOnTrack = budgets.Count(b => b.CurrentSpent <= b.BudgetAmount);
        var budgetsExceeded = totalBudgets - budgetsOnTrack;
        
        var overallUtilization = totalBudgetAmount > 0 ? (totalSpent / totalBudgetAmount) * 100 : 0;

        return new BudgetOverviewDto
        {
            TotalBudgets = totalBudgets,
            BudgetsOnTrack = budgetsOnTrack,
            BudgetsExceeded = budgetsExceeded,
            TotalBudgetAmount = totalBudgetAmount,
            TotalSpent = totalSpent,
            OverallUtilization = overallUtilization
        };
    }

    private async Task<decimal> CalculateSavingsForMonth(Guid userId, DateTime month)
    {
        var startDate = new DateTime(month.Year, month.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId && 
                       t.TransactionDate >= DateOnly.FromDateTime(startDate) && 
                       t.TransactionDate <= DateOnly.FromDateTime(endDate))
            .ToListAsync();

        var income = transactions.Where(t => t.TransactionType == "CREDIT").Sum(t => t.Amount);
        var expenses = transactions.Where(t => t.TransactionType == "DEBIT").Sum(t => t.Amount);

        return income - expenses;
    }

    private async Task<decimal> GetCurrentSavingsAsync(Guid userId)
    {
        var totalBalance = await _context.LinkedAccounts
            .Where(a => a.UserId == userId && a.IsActive)
            .SumAsync(a => a.Balance);

        return totalBalance;
    }

    private static IEnumerable<DailySpendingDto> GetDailySpending(
        IEnumerable<Transaction> transactions, DateTime startDate, DateTime endDate)
    {
        return transactions
            .Where(t => t.TransactionType == "DEBIT")
            .GroupBy(t => t.TransactionDate.ToDateTime(TimeOnly.MinValue).Date)
            .Select(g => new DailySpendingDto
            {
                Date = g.Key,
                Amount = g.Sum(t => t.Amount),
                TransactionCount = g.Count()
            })
            .OrderBy(d => d.Date);
    }

    private static IEnumerable<string> GenerateSpendingInsights(
        IEnumerable<Transaction> expenses, decimal totalExpenses, decimal averageDaily)
    {
        var insights = new List<string>();

        var expensesByDay = expenses.GroupBy(e => e.TransactionDate.ToDateTime(TimeOnly.MinValue).DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        var highestSpendingDay = expensesByDay.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
        if (highestSpendingDay.Value > 0)
        {
            insights.Add($"You spend the most on {highestSpendingDay.Key}s");
        }

        if (averageDaily > 1000)
        {
            insights.Add("Your daily spending is above ₹1,000. Consider tracking smaller expenses.");
        }

        var foodExpenses = expenses.Where(e => e.Category?.Name?.Contains("Food") == true).Sum(e => e.Amount);
        if (foodExpenses > totalExpenses * 0.3m)
        {
            insights.Add("Food expenses account for more than 30% of your spending. Consider meal planning.");
        }

        return insights;
    }
}
