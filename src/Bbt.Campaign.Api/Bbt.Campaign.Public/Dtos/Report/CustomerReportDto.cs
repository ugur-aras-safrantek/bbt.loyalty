using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportDto
    {
        public string? CampaignCode { get; set; }
        public string? CampaignName { get; set; }
        public string? IsBundle { get; set; }
        public string? IsActive { get; set; }
        public string? CustomerNumber { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerType { get; set; }
        public string? BranchCode { get; set; }
        public string? BusinessLine { get; set; }
        public string? EarningType { get; set; }
        public string? CustomerJoinDate { get; set; }
        public decimal? EarningAmount { get; set; }
        public decimal? EarningRate { get; set; }
        public string? IsEarningUsed { get; set; }
        public string? EarningUsedDate { get; set; }
        public string? CampaignStartDate { get; set; }
    }
}
