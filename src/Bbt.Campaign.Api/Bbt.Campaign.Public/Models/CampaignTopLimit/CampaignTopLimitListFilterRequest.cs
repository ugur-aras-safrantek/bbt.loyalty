using Bbt.Campaign.Public.Enums;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.CampaignTopLimit
{
    public class CampaignTopLimitListFilterRequest: PagingRequest
    {
        public string? Name { get; set; }
        public int? AchievementFrequencyId { get; set; }
        public int? CurrencyId { get; set; }
        public decimal? MaxTopLimitAmount { get; set; }
        public decimal? MaxTopLimitRate { get; set; }
        public decimal? MaxTopLimitUtilization { get; set; }
        public int? Type { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsDraft { get; set; }
        public int? StatusId { get; set; }
    }
}
