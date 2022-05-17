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
        public DateTime StartDate { get; set; }
        public string StartDateStr { get; set; }
        public int CampaignId { get; set; }
        public int IsContinuingCampaign { get; set; }
        public DateTime CampaignStartDate { get; set; }
        public string CampaignStartDateStr { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }       
        public int JoinTypeId { get; set; }
        public string? JoinTypeName { get; set; }
        public string? CustomerTypeId { get; set; }
        public string? CustomerTypeName { get; set; }
        public string? CampaignStartTermId { get; set; }
        public string? CampaignStartTermName { get; set; }
        public string? BusinessLineId { get; set; }
        public string? BusinessLineName { get; set; }
    }
}
