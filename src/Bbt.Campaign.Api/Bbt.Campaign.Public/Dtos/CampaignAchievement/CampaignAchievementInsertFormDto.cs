namespace Bbt.Campaign.Public.Dtos.CampaignAchievement
{
    public class CampaignAchievementInsertFormDto
    {
        public bool IsInvisibleCampaign { get; set; }
        public List<ParameterDto> ActionOptions { get; set; }
        public List<ParameterDto> AchievementTypes { get; set; }
        public List<ParameterDto> CurrencyList { get; set; }
    }
}
