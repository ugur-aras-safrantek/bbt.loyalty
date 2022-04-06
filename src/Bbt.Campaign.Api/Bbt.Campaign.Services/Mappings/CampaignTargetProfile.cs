using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.CampaignTarget;

namespace Bbt.Campaign.Services.Mappings
{
    public class CampaignTargetProfile : Profile
    {
        public CampaignTargetProfile()
        {
            CreateMap<CampaignTargetDto, CampaignTargetEntity>().ReverseMap();
            CreateMap<ParameterDto, TargetEntity>().ReverseMap();
        }
    }
}
