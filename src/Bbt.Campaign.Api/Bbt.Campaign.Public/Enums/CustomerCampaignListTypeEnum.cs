using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum CustomerCampaignListTypeEnum
    {
        [Description("Kampanyalar")]
        Campaign = 1,
        [Description("Katıldıklarım")]
        Join = 2,
        [Description("Favorilerim")]
        Favorite = 3,
        [Description("Süresi Geçenler")]
        OverDue = 4,
    }
}
