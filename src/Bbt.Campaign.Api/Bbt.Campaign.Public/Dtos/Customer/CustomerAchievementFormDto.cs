using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.CampaignAchievement;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerAchievementFormDto
    {
        public int CampaignId { get; set; }
        public bool IsAchieved { get; set; }
        public bool LastMonthIsAchieved { get; set; }
        public CampaignDto Campaign { get; set; }
        public string? UsedAmountStr { get; set; }
        public string? UsedAmountCurrencyCode { get; set; }
        public string? TotalAchievementStr { get; set; }
        public string? TotalAchievementCurrencyCode { get; set; }
        public string? PreviousMonthAchievementStr { get; set; }
        public string? PreviousMonthAchievementCurrencyCode { get; set; }
        public string? CurrentMonthAchievementStr { get; set; }
        public string? CurrentMonthAchievementCurrencyCode { get; set; }
        public bool IsInvisibleCampaign { get; set; }
        public string? TargetResultDefinition { get; set; }
        public string CampaignLeftDefinition { get; set; }
        public CampaignTargetDto2 CampaignTarget { get; set; }
        public List<CustomerAchievement> CampaignAchievementList { get; set; }
    }
}
