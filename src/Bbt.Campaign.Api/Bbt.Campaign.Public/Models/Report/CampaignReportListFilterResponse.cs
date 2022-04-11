using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CampaignReportListFilterResponse : PagingResponse
    {
        public CampaignReportListFilterResponse()
        {
            ResponseList = new List<CampaignReportListDto>();
        }
        public List<CampaignReportListDto> ResponseList { get; set; }
    }
}
