using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class TopLimitUpdateFields
    {
        public bool IsNameUpdated { get; set; } = false;
        public bool IsActiveUpdated { get; set; } = false;
        public bool IsAchievementFrequencyIdUpdated { get; set; } = false;
        public bool IsTypeIdUpdated { get; set; } = false;
        public bool IsMaxTopLimitAmountUpdated { get; set; } = false;
        public bool IsCurrencyIdUpdated { get; set; } = false;
        public bool IsMaxTopLimitRateUpdated { get; set; } = false;
        public bool IsMaxTopLimitUtilizationUpdated { get; set; } = false;
        public bool IsTopLimitCampaignsUpdated { get; set; } = false;
    }
}
