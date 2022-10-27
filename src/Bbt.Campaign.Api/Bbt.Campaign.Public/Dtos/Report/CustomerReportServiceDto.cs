using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportServiceDto
    {
        public List<ReportData>? ReportData { get; set; }
        public int TotalCount { get; set; }
    }

    public class ReportData
    {
        public string? CampaignCode { get; set; }
        public string? CampaignName { get; set; }
        public bool IsBundle { get; set; }
        public bool IsActive { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerType { get; set; }
        public string? BranchCode { get; set; }
        public string? BusinessLine { get; set; }
        public string? EarningType { get; set; }
        public string CustomerJoinDate { get; set; }
        public double? EarningAmount { get; set; }
        public double? EarningRate { get; set; }
        public bool IsEarningUsed { get; set; }
        public string? EarningUsedDate { get; set; } 
        public string? EarningReachDate { get; set; }
        public string? CampaignStartDate { get; set; }
        public string Term { get; set; }
    }
}
