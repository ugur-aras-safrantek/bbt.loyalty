using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos.CampaignIdentity;

namespace Bbt.Campaign.Services.Mappings
{
    public class CampaignIdentityProfile : Profile
    {
        public CampaignIdentityProfile() 
        {
            CreateMap<CampaignIdentityEntity, CampaignIdentityDto>().ReverseMap();
        }
    }
}
