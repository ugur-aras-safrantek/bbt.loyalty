using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.CampaignChannelCode
{
    public class CampaignChannelCodeUpdateRequest
    {
        public int CampaignId { get; set; }
        public List<string> CampaignChannelCodeList { get; set; }
    }
}
