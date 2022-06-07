using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Dtos.Approval
{
    public class CampaignRuleApproveFormDto
    {
        public string JoinTypeName { get; set; }
        public string RuleBusinessLines { get; set; }
        public string RuleBranches { get; set; }
        public string RuleCustomerTypes { get; set; }
        public string CampaignStartTermName { get; set; }
    }
}
