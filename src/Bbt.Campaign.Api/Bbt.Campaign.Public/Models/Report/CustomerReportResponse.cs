using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CustomerReportResponse : PagingResponse
    {
        public CustomerReportResponse()
        {
            CustomerCampaignList = new List<CustomerReportListDto>();
        }
        public List<CustomerReportListDto> CustomerCampaignList { get; set; }
    }
}
