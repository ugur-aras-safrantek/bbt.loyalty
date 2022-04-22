using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerAchievementFormDto
    {
        public int CampaignId { get; set; }
        public decimal TotalAchievement { get; set; }
        public string TotalAchievementStr { get; set; }
        public bool IsInvisibleCampaign { get; set; }
        public CampaignDto Campaign { get; set; }
        public CampaignTargetDto CampaignTarget { get; set; }
        public List<CampaignAchievementDto> CampaignAchievementList { get; set; }

        //public GetFileResponse ContractFile { get; set; }
    }
}
