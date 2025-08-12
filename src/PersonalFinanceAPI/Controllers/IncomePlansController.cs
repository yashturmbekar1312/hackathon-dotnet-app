using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Models.DTOs;
using System.Security.Claims;

namespace PersonalFinanceAPI.Controllers
{
    /// <summary>
    /// Controller for managing income plans and related operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class IncomePlansController : ControllerBase
    {
        private readonly IIncomePlanService _incomePlanService;
        private readonly ILogger<IncomePlansController> _logger;

        /// <summary>
        /// Initializes a new instance of the IncomePlansController
        /// </summary>
        /// <param name="incomePlanService">The income plan service</param>
        /// <param name="logger">The logger</param>
        public IncomePlansController(IIncomePlanService incomePlanService, ILogger<IncomePlansController> logger)
        {
            _incomePlanService = incomePlanService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all income plans for the authenticated user
        /// </summary>
        /// <returns>List of income plans</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<IncomePlanDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<IncomePlanDto>>> GetIncomePlans()
        {
            try
            {
                var userId = GetCurrentUserId();
                var incomePlans = await _incomePlanService.GetIncomePlansAsync(userId);
                return Ok(incomePlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income plans for user");
                return StatusCode(500, "An error occurred while retrieving income plans");
            }
        }

        /// <summary>
        /// Gets a specific income plan by ID
        /// </summary>
        /// <param name="id">The income plan ID</param>
        /// <returns>The income plan</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IncomePlanDto), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncomePlanDto>> GetIncomePlan(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var incomePlan = await _incomePlanService.GetIncomePlanAsync(id, userId);
                
                if (incomePlan == null)
                {
                    return NotFound("Income plan not found");
                }

                return Ok(incomePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income plan {IncomePlanId}", id);
                return StatusCode(500, "An error occurred while retrieving the income plan");
            }
        }

        /// <summary>
        /// Creates a new income plan
        /// </summary>
        /// <param name="createDto">The income plan creation data</param>
        /// <returns>The created income plan</returns>
        [HttpPost]
        [ProducesResponseType(typeof(IncomePlanDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IncomePlanDto>> CreateIncomePlan([FromBody] CreateIncomePlanDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var incomePlan = await _incomePlanService.CreateIncomePlanAsync(createDto, userId);
                
                return CreatedAtAction(nameof(GetIncomePlan), new { id = incomePlan.Id }, incomePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating income plan for user");
                return StatusCode(500, "An error occurred while creating the income plan");
            }
        }

        /// <summary>
        /// Updates an existing income plan
        /// </summary>
        /// <param name="id">The income plan ID</param>
        /// <param name="updateDto">The income plan update data</param>
        /// <returns>The updated income plan</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IncomePlanDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncomePlanDto>> UpdateIncomePlan(int id, [FromBody] UpdateIncomePlanDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var incomePlan = await _incomePlanService.UpdateIncomePlanAsync(id, updateDto, userId);
                
                if (incomePlan == null)
                {
                    return NotFound("Income plan not found");
                }

                return Ok(incomePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating income plan {IncomePlanId}", id);
                return StatusCode(500, "An error occurred while updating the income plan");
            }
        }

        /// <summary>
        /// Deletes an income plan
        /// </summary>
        /// <param name="id">The income plan ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteIncomePlan(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _incomePlanService.DeleteIncomePlanAsync(id, userId);
                
                if (!result)
                {
                    return NotFound("Income plan not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting income plan {IncomePlanId}", id);
                return StatusCode(500, "An error occurred while deleting the income plan");
            }
        }

        /// <summary>
        /// Gets all income sources for a specific income plan
        /// </summary>
        /// <param name="planId">The income plan ID</param>
        /// <returns>List of income sources</returns>
        [HttpGet("{planId}/sources")]
        [ProducesResponseType(typeof(IEnumerable<IncomeSourceDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<IncomeSourceDto>>> GetIncomeSources(int planId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var sources = await _incomePlanService.GetIncomeSourcesAsync(planId, userId);
                return Ok(sources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income sources for plan {PlanId}", planId);
                return StatusCode(500, "An error occurred while retrieving income sources");
            }
        }

        /// <summary>
        /// Adds a new income source to an income plan
        /// </summary>
        /// <param name="planId">The income plan ID</param>
        /// <param name="createDto">The income source creation data</param>
        /// <returns>The created income source</returns>
        [HttpPost("{planId}/sources")]
        [ProducesResponseType(typeof(IncomeSourceDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncomeSourceDto>> AddIncomeSource(int planId, [FromBody] CreateIncomeSourceDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var source = await _incomePlanService.AddIncomeSourceAsync(planId, createDto, userId);
                
                if (source == null)
                {
                    return NotFound("Income plan not found");
                }

                return CreatedAtAction(nameof(GetIncomeSources), new { planId = planId }, source);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding income source to plan {PlanId}", planId);
                return StatusCode(500, "An error occurred while adding the income source");
            }
        }

        /// <summary>
        /// Gets all income entries for a specific income source
        /// </summary>
        /// <param name="planId">The income plan ID</param>
        /// <param name="sourceId">The income source ID</param>
        /// <returns>List of income entries</returns>
        [HttpGet("{planId}/sources/{sourceId}/entries")]
        [ProducesResponseType(typeof(IEnumerable<IncomeEntryDto>), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IEnumerable<IncomeEntryDto>>> GetIncomeEntries(int planId, int sourceId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var entries = await _incomePlanService.GetIncomeEntriesAsync(planId, sourceId, userId);
                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income entries for source {SourceId} in plan {PlanId}", sourceId, planId);
                return StatusCode(500, "An error occurred while retrieving income entries");
            }
        }

        /// <summary>
        /// Records a new income entry for an income source
        /// </summary>
        /// <param name="planId">The income plan ID</param>
        /// <param name="sourceId">The income source ID</param>
        /// <param name="createDto">The income entry creation data</param>
        /// <returns>The created income entry</returns>
        [HttpPost("{planId}/sources/{sourceId}/entries")]
        [ProducesResponseType(typeof(IncomeEntryDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<IncomeEntryDto>> RecordIncomeEntry(int planId, int sourceId, [FromBody] CreateIncomeEntryDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = GetCurrentUserId();
                var entry = await _incomePlanService.RecordIncomeEntryAsync(planId, sourceId, createDto, userId);
                
                if (entry == null)
                {
                    return NotFound("Income plan or source not found");
                }

                return CreatedAtAction(nameof(GetIncomeEntries), new { planId = planId, sourceId = sourceId }, entry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording income entry for source {SourceId} in plan {PlanId}", sourceId, planId);
                return StatusCode(500, "An error occurred while recording the income entry");
            }
        }

        /// <summary>
        /// Gets income plan statistics
        /// </summary>
        /// <param name="planId">The income plan ID</param>
        /// <returns>Income plan statistics</returns>
        [HttpGet("{planId}/statistics")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> GetIncomePlanStatistics(int planId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var statistics = await _incomePlanService.GetIncomePlanStatisticsAsync(planId, userId);
                
                if (statistics == null)
                {
                    return NotFound("Income plan not found");
                }

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for income plan {PlanId}", planId);
                return StatusCode(500, "An error occurred while retrieving statistics");
            }
        }

        /// <summary>
        /// Gets the current user ID from the JWT token
        /// </summary>
        /// <returns>The user ID</returns>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }
            return userId;
        }
    }
}
