using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum PageTypeEnum
    {
        [Description("Kampanya")]
        Campaign = 1,
        [Description("Kampanya Kuralı")]
        CampaignRule = 2,
        [Description("Hedef Seçimi")]
        CampaignTarget = 3,
        [Description("Kazanım Kanalı")]
        CampaignChannelCode = 3,
        [Description("Kampanya Kazanımlar")]
        CampaignAchievement = 5,
    }
}
