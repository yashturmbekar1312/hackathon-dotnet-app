using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Application.DTOs;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BudgetsController : ControllerBase
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IBudgetService budgetService, ILogger<BudgetsController> logger)
    {
        _budgetService = budgetService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new budget
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BudgetResponse>> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.CreateBudgetAsync(request, userId);
            
            var response = new BudgetResponse
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };

            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            return StatusCode(500, new { message = "An error occurred while creating the budget" });
        }
    }

    /// <summary>
    /// Get a specific budget
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetResponse>> GetBudget(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.GetBudgetAsync(id, userId);

            if (budget == null)
            {
                return NotFound(new { message = "Budget not found" });
            }

            var response = new BudgetResponse
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget {BudgetId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the budget" });
        }
    }

    /// <summary>
    /// Get all budgets for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetBudgets()
    {
        try
        {
            var userId = GetUserId();
            var budgets = await _budgetService.GetUserBudgetsAsync(userId);

            var response = budgets.Select(budget => new BudgetResponse
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budgets for user");
            return StatusCode(500, new { message = "An error occurred while retrieving budgets" });
        }
    }

    /// <summary>
    /// Update a budget
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetResponse>> UpdateBudget(Guid id, [FromBody] UpdateBudgetRequest request)
    {
        try
        {
            var userId = GetUserId();
            var budget = await _budgetService.UpdateBudgetAsync(id, request, userId);

            var response = new BudgetResponse
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };

            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget {BudgetId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the budget" });
        }
    }

    /// <summary>
    /// Delete a budget
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var success = await _budgetService.DeleteBudgetAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Budget not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting budget {BudgetId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the budget" });
        }
    }

    /// <summary>
    /// Get budget utilization for a specific budget
    /// </summary>
    [HttpGet("{id}/utilization")]
    public async Task<ActionResult<BudgetUtilizationDto>> GetBudgetUtilization(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var utilization = await _budgetService.GetBudgetUtilizationAsync(id, userId);
            return Ok(utilization);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget utilization for {BudgetId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving budget utilization" });
        }
    }

    /// <summary>
    /// Get budget utilizations for all user budgets
    /// </summary>
    [HttpGet("utilizations")]
    public async Task<ActionResult<IEnumerable<BudgetUtilizationDto>>> GetAllBudgetUtilizations()
    {
        try
        {
            var userId = GetUserId();
            var utilizations = await _budgetService.GetAllBudgetUtilizationsAsync(userId);
            return Ok(utilizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving budget utilizations for user");
            return StatusCode(500, new { message = "An error occurred while retrieving budget utilizations" });
        }
    }

    /// <summary>
    /// Get all over-budget budgets for the current user
    /// </summary>
    [HttpGet("over-budget")]
    public async Task<ActionResult<IEnumerable<BudgetResponse>>> GetOverBudgets()
    {
        try
        {
            var userId = GetUserId();
            var budgets = await _budgetService.GetOverBudgetsAsync(userId);

            var response = budgets.Select(budget => new BudgetResponse
            {
                Id = budget.Id,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.Name ?? "Unknown",
                BudgetAmount = budget.BudgetAmount,
                PeriodType = budget.PeriodType,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CurrentSpent = budget.CurrentSpent,
                RemainingAmount = budget.RemainingAmount,
                UtilizationPercentage = budget.UtilizationPercentage,
                IsOverBudget = budget.IsOverBudget,
                IsActive = budget.IsActive,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving over-budget budgets for user");
            return StatusCode(500, new { message = "An error occurred while retrieving over-budget budgets" });
        }
    }

    /// <summary>
    /// Get remaining budget amount for a specific budget
    /// </summary>
    [HttpGet("{id}/remaining")]
    public async Task<ActionResult<decimal>> GetRemainingBudget(Guid id)
    {
        try
        {
            var userId = GetUserId();
            var remaining = await _budgetService.GetRemainingBudgetAsync(id, userId);
            return Ok(new { remainingAmount = remaining });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving remaining budget for {BudgetId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving remaining budget" });
        }
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user token");
        }
        return userId;
    }
}
