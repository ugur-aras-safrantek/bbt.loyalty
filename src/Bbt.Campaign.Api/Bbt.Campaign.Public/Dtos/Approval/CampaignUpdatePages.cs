using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignUpdatePages
    {
        public bool IsCampaignUpdated { get; set; } = false;
        public bool IsCampaignRuleUpdated { get; set; } = false;
        public bool IsCampaignTargetListUpdated { get; set; } = false;
        public bool IsCampaignChannelCodeListUpdated { get; set; } = false;
        public bool IsCampaignAchievementListUpdated { get; set; } = false;
    }
}
