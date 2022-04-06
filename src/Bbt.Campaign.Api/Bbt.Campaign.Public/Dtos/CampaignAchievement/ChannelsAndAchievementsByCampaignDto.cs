namespace Bbt.Campaign.Public.Dtos.CampaignAchievement
{
    public class ChannelsAndAchievementsByCampaignDto
    {
        public string CampaignChannelCode { get; set; }
        public string CampaignChannelName { get; set; }
        public bool HasAchievement { get; set; }
        public List<CampaignAchievementListDto> AchievementList { get; set; }
    }
}
