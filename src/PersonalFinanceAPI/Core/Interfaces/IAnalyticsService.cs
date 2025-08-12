using PersonalFinanceAPI.Models.DTOs.Analytics;

namespace PersonalFinanceAPI.Core.Interfaces;

public interface IAnalyticsService
{
    Task<DashboardDto> GetDashboardAsync(Guid userId);
    Task<MonthlyReportDto> GetMonthlyReportAsync(Guid userId, int year, int month);
    Task<IEnumerable<MonthlyReportDto>> GetMonthlyReportsAsync(Guid userId, int? year = null);
    Task<SavingsSummaryDto> GetSavingsSummaryAsync(Guid userId);
    Task<SavingsProjectionDto> GetSavingsProjectionAsync(Guid userId, int months = 12);
    Task<IEnumerable<CategorySpendingDto>> GetCategorySpendingAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<TransactionTrendDto>> GetTransactionTrendsAsync(Guid userId, string period = "monthly");
    Task<ExpenseAnalysisDto> GetExpenseAnalysisAsync(Guid userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task ComputeMonthlySummaryAsync(Guid userId, DateTime monthYear);
    Task RecalculateUserDataAsync(Guid userId);
}
