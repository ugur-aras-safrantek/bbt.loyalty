using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using Bbt.Campaign.Public.Models.File;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerDetailFormDto
    {
        public int CampaignId { get; set; }
        public bool IsInvisibleCampaign { get; set; }
        public CampaignDto Campaign { get; set; }
        public CampaignTargetDto CampaignTarget { get; set; }
        public CampaignAchievementDto CampaignAchievement { get; set; }
        public GetFileResponse ContractFile { get; set; }
    }
}
