using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum IdentitySubTypeEnum
    {
        [Description("Harcama Koşulsuz Dönem")]
        SpendingUnconditionalPeriod = 1,
        [Description("Destek Harcama Tutarı")]
        SupportSpendingAmount = 2,
    }
}
