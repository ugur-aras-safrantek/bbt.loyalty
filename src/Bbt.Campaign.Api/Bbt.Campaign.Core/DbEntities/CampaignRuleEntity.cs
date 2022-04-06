using Bbt.Campaign.Core.BaseEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bbt.Campaign.Core.DbEntities
{
    public  class CampaignRuleEntity : AuditableEntity
    {
        [ForeignKey("Campaign")]
        public int CampaignId { get; set; }
        public CampaignEntity Campaign { get; set; }

        [ForeignKey("JoinType")]
        public int JoinTypeId { get; set; }
        public JoinTypeEntity JoinType { get; set; }

        [ForeignKey("CampaignStartTerm")]
        public int CampaignStartTermId { get; set; }
        public CampaignStartTermEntity  CampaignStartTerm { get; set; }
        public virtual ICollection<CampaignRuleBranchEntity> Branches { get; set; }
        public virtual ICollection<CampaignRuleBusinessLineEntity>  BusinessLines { get; set; }
        public virtual ICollection<CampaignRuleCustomerTypeEntity>  CustomerTypes { get; set; }
        public virtual ICollection<CampaignRuleIdentityEntity> RuleIdentities { get; set; }
    }
}
