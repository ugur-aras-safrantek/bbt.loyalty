using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignRuleBusinessLineEntity : AuditableEntity
    {
        [ForeignKey("CampaignRule")]
        public int CampaignRuleId { get; set; }
        public CampaignRuleEntity CampaignRule { get; set; }

        [ForeignKey("BusinessLine")]
        public int BusinessLineId { get; set; }
        public BusinessLineEntity  BusinessLine { get; set; }
    }
}
