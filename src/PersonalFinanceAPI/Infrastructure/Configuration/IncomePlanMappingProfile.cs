using AutoMapper;
using PersonalFinanceAPI.Models.DTOs;
using PersonalFinanceAPI.Models.Entities;

namespace PersonalFinanceAPI.Infrastructure.Configuration
{
    /// <summary>
    /// AutoMapper profile for Income Plan related mappings
    /// </summary>
    public class IncomePlanMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the IncomePlanMappingProfile
        /// </summary>
        public IncomePlanMappingProfile()
        {
            // Income Plan mappings
            CreateMap<CreateIncomePlanDto, IncomePlan>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentAmount, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeSources, opt => opt.Ignore())
                .ForMember(dest => dest.Milestones, opt => opt.Ignore());

            CreateMap<IncomePlan, IncomePlanDto>()
                .ForMember(dest => dest.CompletionPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeSourcesCount, opt => opt.Ignore())
                .ForMember(dest => dest.MilestonesCount, opt => opt.Ignore());

            // Income Source mappings
            CreateMap<CreateIncomeSourceDto, IncomeSource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IncomePlanId, opt => opt.Ignore())
                .ForMember(dest => dest.ActualAmount, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IncomePlan, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeEntries, opt => opt.Ignore());

            CreateMap<IncomeSource, IncomeSourceDto>();

            // Income Entry mappings
            CreateMap<CreateIncomeEntryDto, IncomeEntry>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeSourceId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IncomeSource, opt => opt.Ignore());

            CreateMap<IncomeEntry, IncomeEntryDto>();

            // Income Plan Milestone mappings
            CreateMap<IncomePlanMilestone, object>()
                .ForMember("Id", opt => opt.MapFrom(src => src.Id))
                .ForMember("Name", opt => opt.MapFrom(src => src.Name))
                .ForMember("Description", opt => opt.MapFrom(src => src.Description))
                .ForMember("TargetAmount", opt => opt.MapFrom(src => src.TargetAmount))
                .ForMember("TargetDate", opt => opt.MapFrom(src => src.TargetDate))
                .ForMember("IsAchieved", opt => opt.MapFrom(src => src.IsAchieved))
                .ForMember("AchievedDate", opt => opt.MapFrom(src => src.AchievedDate));
        }
    }
}
