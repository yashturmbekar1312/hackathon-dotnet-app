using PersonalFinanceAPI.Models.DTOs;

namespace PersonalFinanceAPI.Core.Interfaces
{
    /// <summary>
    /// Interface for income plan service operations
    /// </summary>
    public interface IIncomePlanService
    {
        /// <summary>
        /// Gets all income plans for a user
        /// </summary>
        /// <param name="userId">The user identifier</param>
        /// <returns>Collection of income plans</returns>
        Task<IEnumerable<IncomePlanDto>> GetIncomePlansAsync(Guid userId);

        /// <summary>
        /// Gets a specific income plan by ID
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>The income plan or null if not found</returns>
        Task<IncomePlanDto?> GetIncomePlanAsync(int planId, Guid userId);

        /// <summary>
        /// Creates a new income plan
        /// </summary>
        /// <param name="createDto">The income plan creation data</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>The created income plan</returns>
        Task<IncomePlanDto> CreateIncomePlanAsync(CreateIncomePlanDto createDto, Guid userId);

        /// <summary>
        /// Updates an existing income plan
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="updateDto">The income plan update data</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>The updated income plan or null if not found</returns>
        Task<IncomePlanDto?> UpdateIncomePlanAsync(int planId, UpdateIncomePlanDto updateDto, Guid userId);

        /// <summary>
        /// Deletes an income plan
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>True if deleted successfully, false if not found</returns>
        Task<bool> DeleteIncomePlanAsync(int planId, Guid userId);

        /// <summary>
        /// Gets all income sources for an income plan
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>Collection of income sources</returns>
        Task<IEnumerable<IncomeSourceDto>> GetIncomeSourcesAsync(int planId, Guid userId);

        /// <summary>
        /// Adds a new income source to an income plan
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="createDto">The income source creation data</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>The created income source or null if plan not found</returns>
        Task<IncomeSourceDto?> AddIncomeSourceAsync(int planId, CreateIncomeSourceDto createDto, Guid userId);

        /// <summary>
        /// Gets all income entries for an income source
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="sourceId">The income source identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>Collection of income entries</returns>
        Task<IEnumerable<IncomeEntryDto>> GetIncomeEntriesAsync(int planId, int sourceId, Guid userId);

        /// <summary>
        /// Records a new income entry for an income source
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="sourceId">The income source identifier</param>
        /// <param name="createDto">The income entry creation data</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>The created income entry or null if plan/source not found</returns>
        Task<IncomeEntryDto?> RecordIncomeEntryAsync(int planId, int sourceId, CreateIncomeEntryDto createDto, Guid userId);

        /// <summary>
        /// Gets statistics for an income plan
        /// </summary>
        /// <param name="planId">The income plan identifier</param>
        /// <param name="userId">The user identifier</param>
        /// <returns>Income plan statistics or null if not found</returns>
        Task<object?> GetIncomePlanStatisticsAsync(int planId, Guid userId);
    }
}
