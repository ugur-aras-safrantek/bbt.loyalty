using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CustomerCampaignReportResponse : PagingResponse
    {
        public CustomerCampaignReportResponse()
        {
            CustomerCampaignList = new List<CustomerCampaignReportListDto>();
        }
        public List<CustomerCampaignReportListDto> CustomerCampaignList { get; set; }
    }
}
