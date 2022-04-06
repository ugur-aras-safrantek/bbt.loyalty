using AutoMapper;
using Bbt.Campaign.Core.DbEntities;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Models.CampaignAchievement;

namespace Bbt.Campaign.Services.Mappings
{
    public class CampaignAchievementProfile : Profile
    {
        public CampaignAchievementProfile()
        {
            CreateMap<CampaignAchievementDto, CampaignAchievementEntity>().ReverseMap();
            CreateMap<CampaignAchievementInsertRequest, CampaignAchievementEntity>().ReverseMap();
            CreateMap<CampaignAchievementUpdateRequest, CampaignAchievementEntity>().ReverseMap();
            CreateMap<CampaignAchievementInsertFormDto, CampaignAchievementEntity>().ReverseMap();
            CreateMap<CampaignAchievementListDto, CampaignAchievementEntity>().ReverseMap();
            CreateMap<CampaignAchievementUpdateFormDto, CampaignAchievementEntity>().ReverseMap();
            CreateMap<ChannelsAndAchievementsByCampaignDto, CampaignAchievementEntity>().ReverseMap();
        }
    }
}
