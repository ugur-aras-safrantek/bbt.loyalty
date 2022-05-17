using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class CustomerReportRequest : PagingRequest
    {
        public string? CustomerCode { get; set; }
        public string? CustomerIdentifier { get; set; }
        public int? CustomerTypeId { get; set; }
        public int? CampaignStartTermId { get; set; }
        public string? BranchCode { get; set; }
        public int? AchievementTypeId { get; set; }
        public int? BusinessLineId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsBundle { get; set; }//JoinDate
    }
}
