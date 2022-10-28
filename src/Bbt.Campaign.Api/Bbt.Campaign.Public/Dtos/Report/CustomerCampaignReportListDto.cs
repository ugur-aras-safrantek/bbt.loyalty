using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Report
{
    public class CustomerReportListAndTotalCount
    {
        public List<CustomerCampaignReportListDto> CustomerReportList{ get; set; }
        public int TotalCount { get; set; }
    }
    public class CustomerCampaignReportListDto
    {
        public string CampaignName { get; set; }
        public string CampaignCode { get; set; }
        public string CustomerId { get; set; }
        public bool IsActive { get; set; }
        public bool IsBundle { get; set; }
        public DateTime CustomerJoinDate { get; set; }
        public DateTime? CustomerExitDate { get; set; }
        public bool IsExited { get; set; }
        public DateTime CampaignStartDate { get; set; }
    }
}
