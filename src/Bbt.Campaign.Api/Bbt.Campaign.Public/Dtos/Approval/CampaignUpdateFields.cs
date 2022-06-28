using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignUpdateFields
    {
        public bool IsNameUpdated { get; set; } = false;
        public bool IsDescriptionTrUpdated { get; set; } = false;
        public bool IsDescriptionEnUpdated { get; set; } = false;
        public bool IsTitleTrUpdated { get; set; } = false;
        public bool IsTitleEnUpdated { get; set; } = false;
        public bool IsStartDateUpdated { get; set; } = false;
        public bool IsEndDateUpdated { get; set; } = false;
        public bool IsOrderUpdated { get; set; } = false;
        public bool IsMaxNumberOfUserUpdated { get; set; } = false;
        public bool IsIsActiveUpdated { get; set; } = false;
        public bool IsIsBundleUpdated { get; set; } = false;
        public bool IsIsContractUpdated { get; set; } = false;
        public bool IsContractIdUpdated { get; set; } = false;
        public bool IsSectorIdUpdated { get; set; } = false;
        public bool IsViewOptionIdUpdated { get; set; } = false;
        public bool IsProgramTypeIdUpdated { get; set; } = false;
        public bool IsParticipationTypeIdUpdated { get; set; } = false;

        //detail
        public bool IsCampaignListImageUrlUpdated { get; set; } = false;
        public bool IsCampaignDetailImageUrlUpdated { get; set; } = false;
        public bool IsSummaryTrUpdated { get; set; } = false;
        public bool IsSummaryEnUpdated { get; set; } = false;
        public bool IsContentTrUpdated { get; set; } = false;
        public bool IsContentEnUpdated { get; set; } = false;
        public bool IsDetailTrUpdated { get; set; } = false;
        public bool IsDetailEnUpdated { get; set; } = false;

        //rule
        
        public bool IsCampaignRuleStartTermIdUpdated { get; set; } = false;
        public bool IsCampaignRuleJoinTypeIdUpdated { get; set; } = false;
        public bool IsIsEmployeeIncludedUpdated { get; set; } = false;
        public bool IsIsPrivateBankingUpdated { get; set; } = false;


        public bool IsRuleBusinessLinesUpdated { get; set; } = false;
        public bool IsRuleBranchesUpdated { get; set; } = false;
        public bool IsRuleCustomerTypesUpdated { get; set; } = false;
        public bool IsRuleDocumentUpdated { get; set; } = false;


        //RuleBusinessLines, RuleBranches, RuleCustomerTypes, RuleDocument

    }
}
