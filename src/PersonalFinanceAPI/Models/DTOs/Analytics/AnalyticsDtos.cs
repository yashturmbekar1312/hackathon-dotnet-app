using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceAPI.Models.DTOs.Analytics;

public class DashboardDto
{
    public Guid UserId { get; set; }
    public decimal TotalBalance { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal MonthlyExpenses { get; set; }
    public decimal MonthlySavings { get; set; }
    public decimal SavingsRate { get; set; }
    public int TotalTransactions { get; set; }
    public int UnreadAlerts { get; set; }
    public int ActiveGoals { get; set; }
    public int PendingSuggestions { get; set; }
    public IEnumerable<CategorySpendingDto> TopExpenseCategories { get; set; } = new List<CategorySpendingDto>();
    public IEnumerable<TransactionDto> RecentTransactions { get; set; } = new List<TransactionDto>();
    public BudgetOverviewDto BudgetOverview { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}

public class MonthlyReportDto
{
    public Guid UserId { get; set; }
    public DateTime MonthYear { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetSavings { get; set; }
    public string? TopExpenseCategory { get; set; }
    public decimal TopExpenseAmount { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTransactionAmount { get; set; }
    public decimal SavingsRate { get; set; }
    public bool IsFinal { get; set; }
    public DateTime ComputedAt { get; set; }
    public IEnumerable<CategorySpendingDto> CategoryBreakdown { get; set; } = new List<CategorySpendingDto>();
    public IEnumerable<DailySpendingDto> DailySpending { get; set; } = new List<DailySpendingDto>();
}

public class SavingsSummaryDto
{
    public Guid UserId { get; set; }
    public decimal CurrentMonthSavings { get; set; }
    public decimal SavingsGoal { get; set; }
    public decimal SavingsProgress { get; set; }
    public decimal AverageMonthlySavings { get; set; }
    public decimal TotalSavings { get; set; }
    public decimal ProjectedAnnualSavings { get; set; }
    public bool OnTrackToMeetGoal { get; set; }
    public decimal DeficitOrSurplus { get; set; }
    public IEnumerable<MonthlySavingsDto> LastSixMonths { get; set; } = new List<MonthlySavingsDto>();
}

public class SavingsProjectionDto
{
    public Guid UserId { get; set; }
    public decimal CurrentSavings { get; set; }
    public decimal MonthlyGoal { get; set; }
    public IEnumerable<ProjectionDto> Projections { get; set; } = new List<ProjectionDto>();
    public DateTime GeneratedAt { get; set; }
}

public class CategorySpendingDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
    public decimal Percentage { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
}

public class TransactionTrendDto
{
    public DateTime Period { get; set; }
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetAmount { get; set; }
    public int TransactionCount { get; set; }
}

public class ExpenseAnalysisDto
{
    public Guid UserId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal AverageDaily { get; set; }
    public decimal AverageWeekly { get; set; }
    public decimal AverageMonthly { get; set; }
    public string TopExpenseCategory { get; set; } = string.Empty;
    public decimal TopExpenseAmount { get; set; }
    public IEnumerable<CategorySpendingDto> CategoryBreakdown { get; set; } = new List<CategorySpendingDto>();
    public IEnumerable<string> SpendingInsights { get; set; } = new List<string>();
}

public class BudgetOverviewDto
{
    public int TotalBudgets { get; set; }
    public int BudgetsOnTrack { get; set; }
    public int BudgetsExceeded { get; set; }
    public decimal TotalBudgetAmount { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal OverallUtilization { get; set; }
}

public class DailySpendingDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int TransactionCount { get; set; }
}

public class MonthlySavingsDto
{
    public DateTime Month { get; set; }
    public decimal Amount { get; set; }
    public decimal Goal { get; set; }
    public decimal Achievement { get; set; }
}

public class ProjectionDto
{
    public DateTime Month { get; set; }
    public decimal ProjectedSavings { get; set; }
    public decimal CumulativeSavings { get; set; }
}

public class TransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string? CategoryName { get; set; }
    public string? MerchantName { get; set; }
}
