using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignDetail;
using Bbt.Campaign.Public.Dtos.Customer;
using Bbt.Campaign.Public.Models.Campaign;


namespace Bbt.Campaign.Services.Mappings
{
    internal class CustomerProfile : Profile
    {
        public CustomerProfile() 
        { 
            CreateMap<CustomerCampaignDto, CustomerCampaignEntity>().ReverseMap();
            CreateMap<CustomerCampaignDto, CustomerCampaignFavoriteEntity>().ReverseMap();
        }
    }
}
