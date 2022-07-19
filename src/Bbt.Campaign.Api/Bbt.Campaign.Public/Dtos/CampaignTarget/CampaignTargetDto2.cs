using Bbt.Campaign.Public.Dtos.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignTarget
{
    public class CampaignTargetDto2
    {
        public bool IsAchieved { get; set; }
        public List<TargetParameterDto2> ProgressBarlist { get; set; }
        public List<TargetParameterDto2> Informationlist { get; set; }
        public string? TargetAmountStr { get; set; }
        public string? TargetAmountCurrencyCode { get; set; }
        public string? RemainAmountStr { get; set; }
        public string? RemainAmountCurrencyCode { get; set; }
        public string? UsedAmountStr { get; set; }
        public string? UsedAmountCurrencyCode { get; set; }
    }
}
