using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Campaign
{
    public class CampaignMinDto
    {
        public int Id { get; set; }
        public string? TitleTr { get; set; }
        public string? TitleEn { get; set; }
        public DateTime EndDate { get; set; }
        public string? CampaignListImageUrl { get; set; }
        public string? CampaignDetailImageUrl { get; set; }

    }
}
