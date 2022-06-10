using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignRuleBranchEntity : AuditableEntity
    {
        [ForeignKey("CampaignRule")]
        public int CampaignRuleId { get; set; }
        public CampaignRuleEntity CampaignRule { get; set; }

        [MaxLength(50)]
        public string BranchCode { get; set; }

        [MaxLength(500)]
        public string BranchName { get; set; }
    }
}
