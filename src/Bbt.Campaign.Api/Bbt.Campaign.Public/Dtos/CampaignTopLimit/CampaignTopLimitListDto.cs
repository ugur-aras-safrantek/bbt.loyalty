namespace Bbt.Campaign.Public.Dtos.CampaignTopLimit
{
    public class CampaignTopLimitListDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AchievementFrequency { get; set; }
        public string Currency { get; set; }
        public decimal? MaxTopLimitAmount { get; set; } = 0;
        public decimal? MaxTopLimitRate { get; set; }
        public decimal? MaxTopLimitUtilization { get; set; }
        public string? MaxTopLimitAmountStr { get; set; }
        public string? MaxTopLimitRateStr { get; set; }
        public string? MaxTopLimitUtilizationStr { get; set; }
        public bool Amount { get; set; }
        public bool Rate { get; set; }
        public bool IsActive { get; set; }
        public string CampaignNames { get; set; }

    }
}
