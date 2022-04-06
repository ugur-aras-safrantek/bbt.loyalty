using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Models.Campaign;

namespace Bbt.Campaign.Services.Mappings
{
    internal class CampaignProfile : Profile
    {
        public CampaignProfile()
        {
            CreateMap<CampaignDto, CampaignEntity>().ReverseMap();
            CreateMap<ParameterDto, CampaignEntity>().ReverseMap();
            CreateMap<CampaignInsertRequest, CampaignEntity>().ReverseMap();
            CreateMap<CampaignDetailEntity, CampaignDetailDto>().ReverseMap();
            CreateMap<CampaignDetailDto, CampaignDetailEntity>().ReverseMap();
            CreateMap<CampaignUpdateRequest, CampaignEntity>().ReverseMap();
        }
    }
}
