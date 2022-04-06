using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos.CampaignRule;

namespace Bbt.Campaign.Services.Mappings
{
    public class CampaignRuleProfile : Profile
    {
        public CampaignRuleProfile()
        {
            CreateMap<CampaignRuleDto, CampaignRuleEntity>().ReverseMap();
            CreateMap<CampaignRuleEntity, CampaignRuleUpdateFormDto> ().ReverseMap();
            CreateMap<CampaignRuleUpdateFormDto, CampaignRuleEntity>().ReverseMap();
        }
    }
}
