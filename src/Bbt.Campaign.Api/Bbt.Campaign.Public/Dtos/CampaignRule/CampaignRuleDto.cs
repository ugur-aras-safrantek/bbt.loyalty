using Bbt.Campaign.Public.Dtos.Campaign;

namespace Bbt.Campaign.Public.Dtos.CampaignRule
{
    public class CampaignRuleDto
    {
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public CampaignDto Campaign { get; set; }

        public string? IdentityNumber { get; set; }
        public bool IsSingleIdentity  { get; set; }
        public int JoinTypeId { get; set; }
        public ParameterDto JoinType { get; set; } 
        public List<int> RuleBusinessLines { get; set; }
        public List<string> RuleBranches { get; set; }
        public List<int> RuleCustomerTypes { get; set; }
        public int CampaignStartTermId { get; set; }
        public ParameterDto CampaignStartTerm { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string? DocumentName { get; set; }
    }
}
