using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Models.CampaignAchievement
{
    public class CampaignAchievementUpdateRequest 
    {
        public int CampaignId { get; set; }
        public List<CampaignAchievementInsertRequest> CampaignAchievementList { get; set; }
    }
}
