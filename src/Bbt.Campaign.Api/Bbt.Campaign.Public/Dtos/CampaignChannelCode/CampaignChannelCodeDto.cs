using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignChannelCode
{
    public class CampaignChannelCodeDto
    {
        public int CampaignId { get; set; }
        public List<string> CampaignChannelCodeList { get; set; }
    }
}
