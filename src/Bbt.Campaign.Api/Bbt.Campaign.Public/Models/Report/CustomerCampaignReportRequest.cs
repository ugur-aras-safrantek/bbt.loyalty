using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CustomerCampaignReportRequest : PagingRequest
    {
        public string? CustomerIdentifier { get; set; }
        public bool? IsActive { get; set; }
        public string? CampaignCode { get; set; }//JoinDate
    }
}
