using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Models.CampaignAchievement
{
    public class CampaignAchievementInsertRequest
    {
        public int CampaignId { get; set; }
        public List<CampaignAchievementFormDto> CampaignAchievementList { get; set; }       
    }
}
