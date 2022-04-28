using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum RoleTypeEnum
    {
        [Description("isLoyaltyCreator")]
        IsLoyaltyCreator = 1,
        [Description("isLoyaltyApprover")]
        IsLoyaltyApprover = 2,
        [Description("isLoyaltyReader")]
        IsLoyaltyReader = 3,
        [Description("isLoyaltyRuleCreator")]
        IsLoyaltyRuleCreator = 4,
        [Description("isLoyaltyRuleApprover")]
        IsLoyaltyRuleApprover = 5
    }
}
