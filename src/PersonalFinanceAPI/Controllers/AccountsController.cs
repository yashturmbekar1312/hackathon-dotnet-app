using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Core.Exceptions;
using PersonalFinanceAPI.Models.DTOs.Accounts;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IAccountService accountService, ILogger<AccountsController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    /// <summary>
    /// Link a new bank account
    /// </summary>
    [HttpPost("link")]
    public async Task<IActionResult> LinkAccount([FromBody] LinkAccountRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var account = await _accountService.LinkAccountAsync(userId, request);

            _logger.LogInformation("Account linked successfully for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "Account linked successfully",
                data = account
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to link account for user");
            return BadRequest(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking account");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while linking the account"
            });
        }
    }

    /// <summary>
    /// Get all linked accounts for the current user
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        try
        {
            var userId = GetCurrentUserId();
            var accounts = await _accountService.GetUserAccountsAsync(userId);

            return Ok(new
            {
                success = true,
                data = accounts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user accounts");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving accounts"
            });
        }
    }

    /// <summary>
    /// Get a specific account by ID
    /// </summary>
    [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> GetAccount(Guid accountId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var account = await _accountService.GetAccountByIdAsync(userId, accountId);

            if (account == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Account not found"
                });
            }

            return Ok(new
            {
                success = true,
                data = account
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving account {AccountId}", accountId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving the account"
            });
        }
    }

    /// <summary>
    /// Update account information
    /// </summary>
    [HttpPut("{accountId:guid}")]
    public async Task<IActionResult> UpdateAccount(Guid accountId, [FromBody] UpdateAccountRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var account = await _accountService.UpdateAccountAsync(userId, accountId, request);

            _logger.LogInformation("Account {AccountId} updated for user {UserId}", accountId, userId);

            return Ok(new
            {
                success = true,
                message = "Account updated successfully",
                data = account
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Account not found for update");
            return NotFound(new
            {
                success = false,
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating account {AccountId}", accountId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while updating the account"
            });
        }
    }

    /// <summary>
    /// Unlink an account
    /// </summary>
    [HttpDelete("{accountId:guid}")]
    public async Task<IActionResult> UnlinkAccount(Guid accountId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _accountService.UnlinkAccountAsync(userId, accountId);

            if (result)
            {
                _logger.LogInformation("Account {AccountId} unlinked for user {UserId}", accountId, userId);

                return Ok(new
                {
                    success = true,
                    message = "Account unlinked successfully"
                });
            }

            return NotFound(new
            {
                success = false,
                message = "Account not found"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlinking account {AccountId}", accountId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while unlinking the account"
            });
        }
    }

    /// <summary>
    /// Sync data for a specific account
    /// </summary>
    [HttpPost("{accountId:guid}/sync")]
    public async Task<IActionResult> SyncAccount(Guid accountId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _accountService.SyncAccountDataAsync(userId, accountId);

            if (result)
            {
                _logger.LogInformation("Account {AccountId} synced for user {UserId}", accountId, userId);

                return Ok(new
                {
                    success = true,
                    message = "Account synced successfully"
                });
            }

            return NotFound(new
            {
                success = false,
                message = "Account not found"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing account {AccountId}", accountId);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while syncing the account"
            });
        }
    }

    /// <summary>
    /// Sync all linked accounts
    /// </summary>
    [HttpPost("sync-all")]
    public async Task<IActionResult> SyncAllAccounts()
    {
        try
        {
            var userId = GetCurrentUserId();
            var accounts = await _accountService.SyncAllAccountsAsync(userId);

            _logger.LogInformation("All accounts synced for user {UserId}", userId);

            return Ok(new
            {
                success = true,
                message = "All accounts synced successfully",
                data = accounts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error syncing all accounts");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while syncing accounts"
            });
        }
    }

    /// <summary>
    /// Get available bank aggregators
    /// </summary>
    [HttpGet("bank-aggregators")]
    public async Task<IActionResult> GetBankAggregators()
    {
        try
        {
            var aggregators = await _accountService.GetBankAggregatorsAsync();

            return Ok(new
            {
                success = true,
                data = aggregators
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bank aggregators");
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred while retrieving bank aggregators"
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
