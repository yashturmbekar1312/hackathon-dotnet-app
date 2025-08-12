using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Core.Interfaces;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IAnalyticsService analyticsService, ILogger<AnalyticsController> logger)
    {
        _analyticsService = analyticsService;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive dashboard data for the current user
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dashboard = await _analyticsService.GetDashboardAsync(userId);

            return Ok(new
            {
                success = true,
                data = dashboard
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving dashboard data"
            });
        }
    }

    /// <summary>
    /// Get monthly report for a specific month
    /// </summary>
    [HttpGet("reports/monthly/{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonthlyReport(int year, int month)
    {
        try
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Month must be between 1 and 12"
                });
            }

            var userId = GetCurrentUserId();
            var report = await _analyticsService.GetMonthlyReportAsync(userId, year, month);

            return Ok(new
            {
                success = true,
                data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly report for {Year}-{Month}", year, month);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving the monthly report"
            });
        }
    }

    /// <summary>
    /// Get all monthly reports for a specific year
    /// </summary>
    [HttpGet("reports/monthly")]
    public async Task<IActionResult> GetMonthlyReports([FromQuery] int? year = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var reports = await _analyticsService.GetMonthlyReportsAsync(userId, year);

            return Ok(new
            {
                success = true,
                data = reports
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monthly reports for year {Year}", year);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving monthly reports"
            });
        }
    }

    /// <summary>
    /// Get savings summary with goals and projections
    /// </summary>
    [HttpGet("savings/summary")]
    public async Task<IActionResult> GetSavingsSummary()
    {
        try
        {
            var userId = GetCurrentUserId();
            var summary = await _analyticsService.GetSavingsSummaryAsync(userId);

            return Ok(new
            {
                success = true,
                data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting savings summary");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving savings summary"
            });
        }
    }

    /// <summary>
    /// Get savings projections for future months
    /// </summary>
    [HttpGet("savings/projections")]
    public async Task<IActionResult> GetSavingsProjections([FromQuery] int months = 12)
    {
        try
        {
            if (months < 1 || months > 60)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Months must be between 1 and 60"
                });
            }

            var userId = GetCurrentUserId();
            var projections = await _analyticsService.GetSavingsProjectionAsync(userId, months);

            return Ok(new
            {
                success = true,
                data = projections
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting savings projections");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving savings projections"
            });
        }
    }

    /// <summary>
    /// Get spending breakdown by category
    /// </summary>
    [HttpGet("spending/categories")]
    public async Task<IActionResult> GetCategorySpending(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var categorySpending = await _analyticsService.GetCategorySpendingAsync(userId, fromDate, toDate);

            return Ok(new
            {
                success = true,
                data = categorySpending
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category spending");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving category spending"
            });
        }
    }

    /// <summary>
    /// Get transaction trends over time
    /// </summary>
    [HttpGet("trends")]
    public async Task<IActionResult> GetTransactionTrends([FromQuery] string period = "monthly")
    {
        try
        {
            var allowedPeriods = new[] { "daily", "weekly", "monthly", "yearly" };
            if (!allowedPeriods.Contains(period.ToLower()))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Period must be one of: daily, weekly, monthly, yearly"
                });
            }

            var userId = GetCurrentUserId();
            var trends = await _analyticsService.GetTransactionTrendsAsync(userId, period);

            return Ok(new
            {
                success = true,
                data = trends
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction trends");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving transaction trends"
            });
        }
    }

    /// <summary>
    /// Get detailed expense analysis
    /// </summary>
    [HttpGet("expenses/analysis")]
    public async Task<IActionResult> GetExpenseAnalysis(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var analysis = await _analyticsService.GetExpenseAnalysisAsync(userId, fromDate, toDate);

            return Ok(new
            {
                success = true,
                data = analysis
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expense analysis");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving expense analysis"
            });
        }
    }

    /// <summary>
    /// Trigger recalculation of user data and analytics
    /// </summary>
    [HttpPost("recalculate")]
    public async Task<IActionResult> RecalculateUserData()
    {
        try
        {
            var userId = GetCurrentUserId();
            await _analyticsService.RecalculateUserDataAsync(userId);

            _logger.LogInformation("Data recalculation triggered for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "Data recalculation completed successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recalculating user data");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while recalculating data"
            });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("user_id")?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Invalid user context");
    }
}
