using Bbt.Campaign.Public.Dtos.CampaignAchievement;

namespace Bbt.Campaign.Public.Models.CampaignAchievement
{
    public class ChannelsAndAchievementsByCampaignResponse
    {
        public int CampaignId { get; set; }
        public ChannelsAndAchievementsByCampaignResponse()
        {
            ChannelsAndAchievements = new List<ChannelsAndAchievementsByCampaignDto>();
        }
        public List<ChannelsAndAchievementsByCampaignDto> ChannelsAndAchievements { get; set; }

    }
}
