using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class TargetReportServiceDto
    {
        public List<TargetReportData>? ReportData { get; set; }
        public int TotalCount { get; set; }
    }

    public class TargetReportData
    {
        public string? CampaignCode { get; set; }
        public string? CampaignName { get; set; }
        public string? TargetCode { get; set; }
        public string? TargetName { get; set; }
        public bool IsJoin { get; set; }    
        public string? CustomerNumber { get; set; }
        public int? SubSegment { get; set; }
        public double? TargetAmount { get; set; }
        public string? TargetAmountCurrency { get; set; }
        public double? RemainingAmount { get; set; }
        public string? RemainingAmountCurrency { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
