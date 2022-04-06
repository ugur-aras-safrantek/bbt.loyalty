using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos.CampaignTopLimit;
using Bbt.Campaign.Public.Models.CampaignTopLimit;

namespace Bbt.Campaign.Services.Mappings
{
    public class CampaignTopLimitProfile : Profile
    {
        public CampaignTopLimitProfile()
        {
            CreateMap<TopLimitDto, TopLimitEntity>().ReverseMap();
            CreateMap<CampaignTopLimitInsertRequest, TopLimitEntity>().ReverseMap();
            CreateMap<CampaignTopLimitUpdateRequest, TopLimitEntity>().ReverseMap();
            CreateMap<CampaignTopLimitListDto, TopLimitEntity>().ReverseMap();
            CreateMap<CampaignTopLimitInsertBaseRequest, TopLimitEntity>().ReverseMap();
        }
    }
}
