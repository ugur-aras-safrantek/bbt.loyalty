using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.CampaignChannelCode
{
    public class CampaignChannelCodeInsertFormDto
    {
        public bool IsInvisibleCampaign { get; set; }
        public List<string> ChannelCodeList { get; set; }
    }
}
