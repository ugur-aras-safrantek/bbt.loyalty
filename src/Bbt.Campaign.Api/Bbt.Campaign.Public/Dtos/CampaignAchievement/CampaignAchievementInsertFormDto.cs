namespace Bbt.Campaign.Public.Dtos.CampaignAchievement
{
    public class CampaignAchievementInsertFormDto
    {
        public List<ParameterDto> ActionOptions { get; set; }
        public List<ParameterDto> AchievementTypes { get; set; }
        public List<ParameterDto> CurrencyList { get; set; }
        public List<string> ChannelCodeList { get; set; }
        public bool IsCampaignInvisible { get; set; }
    }
}
