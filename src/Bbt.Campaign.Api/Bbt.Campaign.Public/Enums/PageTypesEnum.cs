using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum PageTypesEnum
    {
        [Description("Campaign")]
        Campaign = 1,
        [Description("CampaignRule")]
        CampaignRule = 2,
        [Description("CampaignTarget")]
        CampaignTarget = 3,
        [Description("ChannelCode")]
        ChannelCode = 4,
        [Description("CampaignAchievement")]
        CampaignAchievement = 5,
    }
}
