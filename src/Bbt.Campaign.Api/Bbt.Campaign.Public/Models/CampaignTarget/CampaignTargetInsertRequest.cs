using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.CampaignTarget
{
    public class CampaignTargetInsertRequest
    {
        public int CampaignId { get; set; }
        //public List<CampaignTargetInsertRequestModel> CampaignTargetList { get; set; }

        public List<int> TargetList { get; set; }
    }
}
