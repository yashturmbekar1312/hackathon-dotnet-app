using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Core.Interfaces;
using PersonalFinanceAPI.Infrastructure.Data;
using PersonalFinanceAPI.Models.DTOs;
using PersonalFinanceAPI.Models.Entities;

namespace PersonalFinanceAPI.Application.Services
{
    /// <summary>
    /// Service for managing income plans and related operations
    /// </summary>
    public class IncomePlanService : IIncomePlanService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<IncomePlanService> _logger;

        /// <summary>
        /// Initializes a new instance of the IncomePlanService
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="mapper">The AutoMapper instance</param>
        /// <param name="logger">The logger</param>
        public IncomePlanService(AppDbContext context, IMapper mapper, ILogger<IncomePlanService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IncomePlanDto>> GetIncomePlansAsync(Guid userId)
        {
            try
            {
                var incomePlans = await _context.IncomePlans
                    .Where(ip => ip.UserId == userId)
                    .Include(ip => ip.IncomeSources)
                    .Include(ip => ip.Milestones)
                    .OrderByDescending(ip => ip.CreatedAt)
                    .ToListAsync();

                var incomePlanDtos = _mapper.Map<List<IncomePlanDto>>(incomePlans);

                // Calculate additional properties
                foreach (var dto in incomePlanDtos)
                {
                    var plan = incomePlans.First(ip => ip.Id == dto.Id);
                    dto.CompletionPercentage = plan.TargetAmount > 0 ? 
                        Math.Round((plan.CurrentAmount / plan.TargetAmount) * 100, 2) : 0;
                    dto.IncomeSourcesCount = plan.IncomeSources.Count;
                    dto.MilestonesCount = plan.Milestones.Count;
                }

                return incomePlanDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income plans for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IncomePlanDto?> GetIncomePlanAsync(int planId, Guid userId)
        {
            try
            {
                var incomePlan = await _context.IncomePlans
                    .Where(ip => ip.Id == planId && ip.UserId == userId)
                    .Include(ip => ip.IncomeSources)
                    .Include(ip => ip.Milestones)
                    .FirstOrDefaultAsync();

                if (incomePlan == null)
                {
                    return null;
                }

                var dto = _mapper.Map<IncomePlanDto>(incomePlan);
                dto.CompletionPercentage = incomePlan.TargetAmount > 0 ? 
                    Math.Round((incomePlan.CurrentAmount / incomePlan.TargetAmount) * 100, 2) : 0;
                dto.IncomeSourcesCount = incomePlan.IncomeSources.Count;
                dto.MilestonesCount = incomePlan.Milestones.Count;

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income plan {PlanId} for user {UserId}", planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IncomePlanDto> CreateIncomePlanAsync(CreateIncomePlanDto createDto, Guid userId)
        {
            try
            {
                var incomePlan = _mapper.Map<IncomePlan>(createDto);
                incomePlan.UserId = userId;
                incomePlan.CreatedAt = DateTime.UtcNow;
                incomePlan.UpdatedAt = DateTime.UtcNow;

                _context.IncomePlans.Add(incomePlan);
                await _context.SaveChangesAsync();

                var dto = _mapper.Map<IncomePlanDto>(incomePlan);
                dto.CompletionPercentage = 0;
                dto.IncomeSourcesCount = 0;
                dto.MilestonesCount = 0;

                _logger.LogInformation("Created income plan {PlanId} for user {UserId}", incomePlan.Id, userId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating income plan for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IncomePlanDto?> UpdateIncomePlanAsync(int planId, UpdateIncomePlanDto updateDto, Guid userId)
        {
            try
            {
                var incomePlan = await _context.IncomePlans
                    .Where(ip => ip.Id == planId && ip.UserId == userId)
                    .Include(ip => ip.IncomeSources)
                    .Include(ip => ip.Milestones)
                    .FirstOrDefaultAsync();

                if (incomePlan == null)
                {
                    return null;
                }

                // Update only provided fields
                if (!string.IsNullOrEmpty(updateDto.Name))
                    incomePlan.Name = updateDto.Name;
                
                if (updateDto.Description != null)
                    incomePlan.Description = updateDto.Description;
                
                if (updateDto.TargetAmount.HasValue)
                    incomePlan.TargetAmount = updateDto.TargetAmount.Value;
                
                if (!string.IsNullOrEmpty(updateDto.PlanType))
                    incomePlan.PlanType = updateDto.PlanType;
                
                if (updateDto.EndDate.HasValue)
                    incomePlan.EndDate = updateDto.EndDate;
                
                if (!string.IsNullOrEmpty(updateDto.Status))
                    incomePlan.Status = updateDto.Status;
                
                if (updateDto.Priority.HasValue)
                    incomePlan.Priority = updateDto.Priority.Value;
                
                if (updateDto.Notes != null)
                    incomePlan.Notes = updateDto.Notes;

                incomePlan.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var dto = _mapper.Map<IncomePlanDto>(incomePlan);
                dto.CompletionPercentage = incomePlan.TargetAmount > 0 ? 
                    Math.Round((incomePlan.CurrentAmount / incomePlan.TargetAmount) * 100, 2) : 0;
                dto.IncomeSourcesCount = incomePlan.IncomeSources.Count;
                dto.MilestonesCount = incomePlan.Milestones.Count;

                _logger.LogInformation("Updated income plan {PlanId} for user {UserId}", planId, userId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating income plan {PlanId} for user {UserId}", planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteIncomePlanAsync(int planId, Guid userId)
        {
            try
            {
                var incomePlan = await _context.IncomePlans
                    .Where(ip => ip.Id == planId && ip.UserId == userId)
                    .FirstOrDefaultAsync();

                if (incomePlan == null)
                {
                    return false;
                }

                _context.IncomePlans.Remove(incomePlan);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted income plan {PlanId} for user {UserId}", planId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting income plan {PlanId} for user {UserId}", planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IncomeSourceDto>> GetIncomeSourcesAsync(int planId, Guid userId)
        {
            try
            {
                var sources = await _context.IncomeSources
                    .Where(s => s.IncomePlanId == planId && s.IncomePlan.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync();

                return _mapper.Map<List<IncomeSourceDto>>(sources);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income sources for plan {PlanId} and user {UserId}", planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IncomeSourceDto?> AddIncomeSourceAsync(int planId, CreateIncomeSourceDto createDto, Guid userId)
        {
            try
            {
                // Verify the income plan exists and belongs to the user
                var planExists = await _context.IncomePlans
                    .AnyAsync(ip => ip.Id == planId && ip.UserId == userId);

                if (!planExists)
                {
                    return null;
                }

                var incomeSource = _mapper.Map<IncomeSource>(createDto);
                incomeSource.IncomePlanId = planId;
                incomeSource.CreatedAt = DateTime.UtcNow;
                incomeSource.UpdatedAt = DateTime.UtcNow;

                _context.IncomeSources.Add(incomeSource);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added income source {SourceId} to plan {PlanId} for user {UserId}", 
                    incomeSource.Id, planId, userId);

                return _mapper.Map<IncomeSourceDto>(incomeSource);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding income source to plan {PlanId} for user {UserId}", planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<IncomeEntryDto>> GetIncomeEntriesAsync(int planId, int sourceId, Guid userId)
        {
            try
            {
                var entries = await _context.IncomeEntries
                    .Where(e => e.IncomeSourceId == sourceId && 
                               e.IncomeSource.IncomePlanId == planId && 
                               e.IncomeSource.IncomePlan.UserId == userId)
                    .OrderByDescending(e => e.ReceivedDate)
                    .ToListAsync();

                return _mapper.Map<List<IncomeEntryDto>>(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving income entries for source {SourceId} in plan {PlanId} for user {UserId}", 
                    sourceId, planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IncomeEntryDto?> RecordIncomeEntryAsync(int planId, int sourceId, CreateIncomeEntryDto createDto, Guid userId)
        {
            try
            {
                // Verify the income source exists and belongs to the user's plan
                var incomeSource = await _context.IncomeSources
                    .Where(s => s.Id == sourceId && 
                               s.IncomePlanId == planId && 
                               s.IncomePlan.UserId == userId)
                    .Include(s => s.IncomePlan)
                    .FirstOrDefaultAsync();

                if (incomeSource == null)
                {
                    return null;
                }

                var incomeEntry = _mapper.Map<IncomeEntry>(createDto);
                incomeEntry.IncomeSourceId = sourceId;
                incomeEntry.CreatedAt = DateTime.UtcNow;
                incomeEntry.UpdatedAt = DateTime.UtcNow;

                _context.IncomeEntries.Add(incomeEntry);

                // Update the actual amount in the income source
                incomeSource.ActualAmount += incomeEntry.Amount;
                incomeSource.UpdatedAt = DateTime.UtcNow;

                // Update the current amount in the income plan
                incomeSource.IncomePlan.CurrentAmount += incomeEntry.Amount;
                incomeSource.IncomePlan.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Recorded income entry {EntryId} for source {SourceId} in plan {PlanId} for user {UserId}", 
                    incomeEntry.Id, sourceId, planId, userId);

                return _mapper.Map<IncomeEntryDto>(incomeEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording income entry for source {SourceId} in plan {PlanId} for user {UserId}", 
                    sourceId, planId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<object?> GetIncomePlanStatisticsAsync(int planId, Guid userId)
        {
            try
            {
                var incomePlan = await _context.IncomePlans
                    .Where(ip => ip.Id == planId && ip.UserId == userId)
                    .Include(ip => ip.IncomeSources)
                        .ThenInclude(s => s.IncomeEntries)
                    .Include(ip => ip.Milestones)
                    .FirstOrDefaultAsync();

                if (incomePlan == null)
                {
                    return null;
                }

                var totalEntries = incomePlan.IncomeSources.SelectMany(s => s.IncomeEntries).Count();
                var totalExpectedAmount = incomePlan.IncomeSources.Sum(s => s.ExpectedAmount);
                var totalActualAmount = incomePlan.IncomeSources.Sum(s => s.ActualAmount);
                var completionPercentage = incomePlan.TargetAmount > 0 ? 
                    Math.Round((incomePlan.CurrentAmount / incomePlan.TargetAmount) * 100, 2) : 0;

                var sourceBreakdown = incomePlan.IncomeSources.Select(s => new
                {
                    SourceName = s.Name,
                    SourceType = s.SourceType,
                    ExpectedAmount = s.ExpectedAmount,
                    ActualAmount = s.ActualAmount,
                    EntriesCount = s.IncomeEntries.Count,
                    CompletionPercentage = s.ExpectedAmount > 0 ? 
                        Math.Round((s.ActualAmount / s.ExpectedAmount) * 100, 2) : 0
                }).ToList();

                var statistics = new
                {
                    PlanId = incomePlan.Id,
                    PlanName = incomePlan.Name,
                    TargetAmount = incomePlan.TargetAmount,
                    CurrentAmount = incomePlan.CurrentAmount,
                    CompletionPercentage = completionPercentage,
                    TotalExpectedAmount = totalExpectedAmount,
                    TotalActualAmount = totalActualAmount,
                    TotalIncomeEntries = totalEntries,
                    IncomeSourcesCount = incomePlan.IncomeSources.Count,
                    ActiveSourcesCount = incomePlan.IncomeSources.Count(s => s.IsActive),
                    MilestonesCount = incomePlan.Milestones.Count,
                    AchievedMilestonesCount = incomePlan.Milestones.Count(m => m.IsAchieved),
                    DaysActive = (DateTime.UtcNow - incomePlan.StartDate).Days,
                    DaysRemaining = incomePlan.EndDate.HasValue ? 
                        Math.Max(0, (incomePlan.EndDate.Value - DateTime.UtcNow).Days) : (int?)null,
                    SourceBreakdown = sourceBreakdown
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics for income plan {PlanId} and user {UserId}", planId, userId);
                throw;
            }
        }
    }
}
