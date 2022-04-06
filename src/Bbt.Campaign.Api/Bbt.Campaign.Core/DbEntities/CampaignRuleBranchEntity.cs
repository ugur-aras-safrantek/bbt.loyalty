using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public class CampaignRuleBranchEntity : AuditableEntity
    {
        [ForeignKey("CampaignRule")]
        public int CampaignRuleId { get; set; }
        public CampaignRuleEntity CampaignRule { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }
}
