using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignRuleCustomerTypeEntity : AuditableEntity
    {
        [ForeignKey("CampaignRule")]
        public int CampaignRuleId { get; set; }
        public CampaignRuleEntity  CampaignRule { get; set; }

        [ForeignKey("CustomerType")]
        public int CustomerTypeId { get; set; }
        public CustomerTypeEntity  CustomerType { get; set; }
    }
}
