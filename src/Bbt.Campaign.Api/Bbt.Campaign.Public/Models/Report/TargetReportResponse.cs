using Bbt.Campaign.Public.Dtos.Report;
using Bbt.Campaign.Public.Models.Paging;

namespace Bbt.Campaign.Public.Models.Report
{
    public class TargetReportResponse : PagingResponse
    {
        public TargetReportResponse()
        {
            TargetReportList = new List<TargetReportListDto>();
        }
        public List<TargetReportListDto> TargetReportList { get; set; }
    }
}
