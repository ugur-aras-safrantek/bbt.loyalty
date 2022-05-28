using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Authorization;

namespace Bbt.Campaign.Services.Mappings
{
    internal class ParameterProfile : Profile
    {
        public ParameterProfile()
        {
            CreateMap<ParameterDto, ActionOptionEntity>().ReverseMap();
            CreateMap<ParameterDto, BusinessLineEntity>().ReverseMap();
            CreateMap<ParameterDto, CampaignStartTermEntity>().ReverseMap();
            CreateMap<ParameterDto, CustomerTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, JoinTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, LanguageEntity>().ReverseMap();
            CreateMap<ParameterDto, SectorEntity>().ReverseMap();
            CreateMap<ParameterDto, ViewOptionEntity>().ReverseMap();
            CreateMap<ParameterDto, AchievementFrequencyEntity>().ReverseMap();
            CreateMap<ParameterDto, CurrencyEntity>().ReverseMap();
            CreateMap<ParameterDto, TargetOperationEntity>().ReverseMap();
            CreateMap<ParameterDto, ProgramTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, AchievementTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, TargetSourceEntity>().ReverseMap();
            CreateMap<ParameterDto, TriggerTimeEntity>().ReverseMap();
            CreateMap<ParameterDto, TargetViewTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, VerificationTimeEntity>().ReverseMap();
            CreateMap<ParameterDto, ParticipationTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, RoleTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, UserRoleEntity>().ReverseMap();
            CreateMap<UserRoleDto, UserRoleEntity>().ReverseMap();
            CreateMap<ParameterDto, UserRoleDto>().ReverseMap();
            CreateMap<ParameterDto, ModuleTypeEntity>().ReverseMap();
            CreateMap<ParameterDto, AuthorizationTypeEntity>().ReverseMap();
            CreateMap<RoleAuthorizationDto, RoleAuthorizationEntity>().ReverseMap();
            CreateMap<ParameterDto, ConstantsEntity>().ReverseMap();
            CreateMap<ParameterDto, StatusEntity>().ReverseMap();
        }
    }
}
