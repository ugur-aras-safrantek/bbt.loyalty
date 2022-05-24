using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportDetailDto
    {
        public string? CampaignCode { get; set; } //"campaignCode": "1",
        public string? CampaignName { get; set; }  //"campaignName": "Demo",
        public bool? IsActive { get; set; } //"isBundle": false,
        public bool? IsBundle { get; set; } //"isActive": true,
        public string? CustomerNumber { get; set; } //"customerNumber": "20070101",
        public string? CustomerId { get; set; } //"customerId": "12345678910",
        public string? CustomerType { get; set; } //"customerType": "Gercek",
        public string? BranchCode { get; set; } //"branchCode": "2000",
        public string? BusinessLine { get; set; } //"businessLine": "X",
        public string? EarningType { get; set; } //"earningType": "Cashback",
        public DateTime? CustomerJoinDate { get; set; } //"customerJoinDate": "2022-05-07T00:00:00",
        public string? CustomerJoinDateStr { get; set; }
        public decimal? EarningAmount { get; set; } //"earningAmount": 30,
        public string? EarningAmountStr { get; set; } //"earningAmount": 30,
        public decimal? EarningRate { get; set; } //"earningRate": null,
        public string? EarningRateStr { get; set; } //"earningRate": null,
        public bool? IsEarningUsed { get; set; } //"isEarningUsed": true,
        public DateTime? EarningUsedDate { get; set; } //"earningUsedDate": "2022-05-18T00:00:00",
        public string? EarningUsedDateStr { get; set; }
        public DateTime? CampaignStartDate { get; set; }  //"campaignStartDate": "2022-05-01T00:00:00" 
        public string? CampaignStartDateStr { get; set; }
    }
}
