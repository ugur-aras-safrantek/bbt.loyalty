using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class TargetReportRequest : PagingRequest
    {
        public int? CampaignId { get; set; }
        public int TargetId { get; set; }
        public int? IdentitySubTypeId { get; set; }
        public bool? IsJoin { get; set; }
        public string? CustomerCode { get; set; }
        public string? TargetSuccessStartDate { get; set; }
        public string? TargetSuccessEndDate { get; set; }
    }
}
