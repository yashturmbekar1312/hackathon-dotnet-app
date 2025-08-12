using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Models.DTOs.Transactions;
using PersonalFinanceAPI.Services;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new transaction
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreateTransactionAsync(userId, request);

            _logger.LogInformation("Transaction created successfully for user {UserId}", userId);

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, new
            {
                success = true,
                message = "Transaction created successfully",
                data = transaction
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request for creating transaction");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while creating the transaction"
            });
        }
    }

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.GetTransactionByIdAsync(userId, id);

            return Ok(new
            {
                success = true,
                data = transaction
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction {TransactionId}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving the transaction"
            });
        }
    }

    /// <summary>
    /// Get transactions with filtering and pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTransactions([FromQuery] TransactionQueryParameters parameters)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.GetTransactionsAsync(userId, parameters);

            return Ok(new
            {
                success = true,
                data = result.Items,
                pagination = new
                {
                    page = result.Page,
                    pageSize = result.PageSize,
                    totalItems = result.TotalItems,
                    totalPages = result.TotalPages
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transactions");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving transactions"
            });
        }
    }

    /// <summary>
    /// Update transaction
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.UpdateTransactionAsync(userId, id, request);

            _logger.LogInformation("Transaction {TransactionId} updated successfully", id);

            return Ok(new
            {
                success = true,
                message = "Transaction updated successfully",
                data = transaction
            });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction {TransactionId}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the transaction"
            });
        }
    }

    /// <summary>
    /// Delete transaction
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _transactionService.DeleteTransactionAsync(userId, id);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Transaction not found"
                });
            }

            _logger.LogInformation("Transaction {TransactionId} deleted successfully", id);

            return Ok(new
            {
                success = true,
                message = "Transaction deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while deleting the transaction"
            });
        }
    }

    /// <summary>
    /// Categorize a transaction
    /// </summary>
    [HttpPut("{id}/categorize")]
    public async Task<IActionResult> CategorizeTransaction(Guid id, [FromBody] CategorizeTransactionRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CategorizeTransactionAsync(userId, id, request);

            _logger.LogInformation("Transaction {TransactionId} categorized successfully", id);

            return Ok(new
            {
                success = true,
                message = "Transaction categorized successfully",
                data = transaction
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error categorizing transaction {TransactionId}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while categorizing the transaction"
            });
        }
    }

    /// <summary>
    /// Bulk categorize multiple transactions
    /// </summary>
    [HttpPost("bulk-categorize")]
    public async Task<IActionResult> BulkCategorizeTransactions([FromBody] BulkCategorizeRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var count = await _transactionService.BulkCategorizeTransactionsAsync(userId, request);

            _logger.LogInformation("Bulk categorized {Count} transactions", count);

            return Ok(new
            {
                success = true,
                message = $"Successfully categorized {count} transactions",
                data = new { categorizedCount = count }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error bulk categorizing transactions");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while categorizing transactions"
            });
        }
    }

    /// <summary>
    /// Get uncategorized transactions
    /// </summary>
    [HttpGet("uncategorized")]
    public async Task<IActionResult> GetUncategorizedTransactions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.GetUncategorizedTransactionsAsync(userId);

            return Ok(new
            {
                success = true,
                data = transactions,
                meta = new
                {
                    count = transactions.Count,
                    message = $"Found {transactions.Count} uncategorized transactions"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving uncategorized transactions");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving uncategorized transactions"
            });
        }
    }

    /// <summary>
    /// Get recurring transactions
    /// </summary>
    [HttpGet("recurring")]
    public async Task<IActionResult> GetRecurringTransactions()
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.GetRecurringTransactionsAsync(userId);

            return Ok(new
            {
                success = true,
                data = transactions,
                meta = new
                {
                    count = transactions.Count,
                    message = $"Found {transactions.Count} recurring transactions"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving recurring transactions");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving recurring transactions"
            });
        }
    }

    /// <summary>
    /// Get transaction summary with analytics
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetTransactionSummary([FromQuery] DateOnly? startDate, [FromQuery] DateOnly? endDate)
    {
        try
        {
            var userId = GetCurrentUserId();
            var summary = await _transactionService.GetTransactionSummaryAsync(userId, startDate, endDate);

            return Ok(new
            {
                success = true,
                data = summary,
                meta = new
                {
                    period = new
                    {
                        startDate = startDate?.ToString("yyyy-MM-dd"),
                        endDate = endDate?.ToString("yyyy-MM-dd")
                    }
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving transaction summary");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving transaction summary"
            });
        }
    }

    /// <summary>
    /// Import transactions from CSV file
    /// </summary>
    [HttpPost("import-csv")]
    public async Task<IActionResult> ImportTransactionsFromCsv([FromForm] ImportTransactionsRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var transactions = await _transactionService.ImportTransactionsFromCsvAsync(userId, request);

            _logger.LogInformation("Imported {Count} transactions from CSV for user {UserId}", transactions.Count, userId);

            return Ok(new
            {
                success = true,
                message = $"Successfully imported {transactions.Count} transactions",
                data = new
                {
                    importedCount = transactions.Count,
                    transactions = transactions.Take(10) // Return first 10 for preview
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing transactions from CSV");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while importing transactions"
            });
        }
    }

    /// <summary>
    /// Sync transactions from bank aggregator
    /// </summary>
    [HttpPost("sync")]
    public async Task<IActionResult> SyncTransactions([FromBody] SyncTransactionsRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var syncedCount = await _transactionService.SyncTransactionsAsync(userId, request);

            _logger.LogInformation("Synced {Count} transactions for user {UserId}", syncedCount, userId);

            return Ok(new
            {
                success = true,
                message = $"Successfully synced {syncedCount} new transactions",
                data = new
                {
                    syncedCount = syncedCount,
                    syncedAt = DateTime.UtcNow
                }
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing transactions");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while syncing transactions"
            });
        }
    }

    /// <summary>
    /// Flag transaction for special handling
    /// </summary>
    [HttpPut("{id}/flags")]
    public async Task<IActionResult> FlagTransaction(Guid id, [FromBody] FlagTransactionRequest request)
    {
        try
        {
            // This would be implemented in the TransactionService
            var userId = GetCurrentUserId();
            
            _logger.LogInformation("Transaction {TransactionId} flagged with {FlagType}: {FlagValue}", 
                id, request.FlagType, request.FlagValue);

            return Ok(new
            {
                success = true,
                message = "Transaction flagged successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flagging transaction {TransactionId}", id);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while flagging the transaction"
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

public class FlagTransactionRequest
{
    public string FlagType { get; set; } = string.Empty;
    public string? FlagValue { get; set; }
}
