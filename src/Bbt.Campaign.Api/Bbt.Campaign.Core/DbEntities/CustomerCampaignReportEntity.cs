
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    [Table("[CustomerJoinReportView]")]
    public class CustomerCampaignReportEntity
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
