using Bbt.Campaign.Public.Dtos.Campaign;
using Bbt.Campaign.Public.Dtos.CampaignAchievement;
using Bbt.Campaign.Public.Dtos.CampaignTarget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Customer
{
    public class CustomerViewFormMinDto
    {
        public int CampaignId { get; set; }
        public bool IsInvisibleCampaign { get; set; }
        public CampaignDto Campaign { get; set; }
        public CampaignTargetDto CampaignTarget { get; set; }
        public CampaignAchievementDto CampaignAchievement { get; set; }
    }
}
