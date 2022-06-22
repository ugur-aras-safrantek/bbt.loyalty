using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportListDto
    {
        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerIdentifier { get; set; }
        public DateTime JoinDate { get; set; }
        public string JoinDateStr { get; set; }
        public int CampaignId { get; set; }
        public string CampaignCode { get; set; }
        public string CampaignName { get; set; }
        public bool IsContinuingCampaign { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public string CampaignStartDateStr { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public int JoinTypeId { get; set; }
        public string? JoinTypeName { get; set; }
        public string? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
        public int CampaignStartTermId { get; set; }
        public string? CampaignStartTermName { get; set; }
        public string? BusinessLineId { get; set; }
        public string? BusinessLineName { get; set; }
        public string? BranchCode { get; set; }
        public string? BranchName { get; set; }
        public string? AchievementTypeId { get; set; }
        public string? AchievementTypeName { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string AchievementDateStr { get; set; }
        public string? AchievementAmountStr { get; set; }
        public string? AchievementRateStr { get; set; }
        public string? EarningReachDateStr { get; set; }
    }
}
