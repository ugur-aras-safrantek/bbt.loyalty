namespace Bbt.Campaign.Public.Dtos.CampaignAchievement
{
    public class CampaignAchievementUpdateFormDto : CampaignAchievementInsertFormDto
    {
        public int CampaignId { get; set; }
        public List<CampaignAchievementDto> CampaignAchievementList { get; set; }
    }
}
