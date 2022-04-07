using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Models.CampaignTopLimit
{
    public class CampaignTopLimitInsertBaseRequest
    {
        public string Name { get; set; }
        public int AchievementFrequencyId { get; set; }
        public int? CurrencyId { get; set; }
        public string? MaxTopLimitAmount { get; set; }
        public string? MaxTopLimitRate { get; set; }
        public string? MaxTopLimitUtilization { get; set; }
        public TopLimitType Type { get; set; }
        public bool IsActive { get; set; }
        public List<int> CampaignIds { get; set; }


    }
}
