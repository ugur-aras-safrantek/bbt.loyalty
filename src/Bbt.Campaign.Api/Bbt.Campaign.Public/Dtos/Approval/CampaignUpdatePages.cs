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
        public bool IsCampaignTargetUpdated { get; set; } = false;
        public bool IsCampaignChannelCodeUpdated { get; set; } = false;
        public bool IsCampaignAchievementUpdated { get; set; } = false;
    }
}
