using Bbt.Campaign.Public.Enums;

namespace Bbt.Campaign.Public.Models.CampaignTopLimit
{
    public class CampaignTopLimitInsertBaseRequest
    {
        public string Name { get; set; }
        public int AchievementFrequencyId { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? MaxTopLimitAmount { get; set; }
        public decimal? MaxTopLimitRate { get; set; }
        public decimal? MaxTopLimitUtilization { get; set; }
        public TopLimitType Type { get; set; }
        public bool IsActive { get; set; }
        public List<int> CampaignIds { get; set; }


    }
}
