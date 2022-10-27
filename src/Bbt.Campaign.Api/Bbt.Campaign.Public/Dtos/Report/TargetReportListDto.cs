using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class TargetReportResp {
        public List<TargetReportListDto> TargetReportList { get; set; }
        public int TotalCount { get; set; }
    }
    public class TargetReportListDto
    {
        public string TargetName { get; set; }
        public string CampaignName { get; set; }
        public bool IsJoin { get; set; }
        public string CustomerCode { get; set; }
        public string? IdentitySubTypeName { get; set; }
        public double? TargetAmount { get; set; }
        public string? TargetAmountStr { get; set; }
        public string? TargetAmountCurrency { get; set; }
        public bool IsTargetSuccess { get; set; }
        public double? RemainAmount { get; set; }
        public string? RemainAmountStr { get; set; }
        public string? RemainAmountCurrency { get; set; }
        public string? TargetSuccessDateStr { get; set; }
        public string Term { get; set; }
    }
}
