using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Target;
using Bbt.Campaign.Public.Dtos.Target.Detail;
using Bbt.Campaign.Public.Dtos.Target.Group;
using Bbt.Campaign.Public.Dtos.Target.Source;
using Bbt.Campaign.Public.Dtos.Target.TriggerTime;
using Bbt.Campaign.Public.Dtos.Target.VerificationTime;
using Bbt.Campaign.Public.Dtos.Target.ViewType;
using Bbt.Campaign.Public.Models.Target;
using Bbt.Campaign.Public.Models.Target.Detail;

namespace Bbt.Campaign.Services.Mappings
{
    internal class TargetProfile : Profile
    {
        public TargetProfile()
        {
            CreateMap<TargetDto, TargetEntity>().ReverseMap();
            CreateMap<TargetInsertRequest, TargetEntity>().ReverseMap();
            CreateMap<TargetDetailDto, TargetDetailEntity>().ReverseMap();
            CreateMap<TargetDetailInsertRequest, TargetDetailEntity>().ReverseMap();
            CreateMap<TargetSourceDto, TargetSourceEntity>().ReverseMap();
            CreateMap<TriggerTimeDto, TriggerTimeEntity>().ReverseMap();
            CreateMap<TargetViewTypeDto, TargetViewTypeEntity>().ReverseMap();
            CreateMap<VerificationTimeDto, VerificationTimeEntity>().ReverseMap();
            CreateMap<TargetGroupDto, TargetGroupEntity>().ReverseMap();
            CreateMap<TargetGroupLineDto, TargetGroupLineEntity>().ReverseMap();
            CreateMap<TargetDto, ParameterDto>().ReverseMap();
            CreateMap<TargetDetailDto, ParameterDto>().ReverseMap();
            CreateMap<TargetDetailDto, TargetEntity>().ReverseMap();
        }
    }
}
