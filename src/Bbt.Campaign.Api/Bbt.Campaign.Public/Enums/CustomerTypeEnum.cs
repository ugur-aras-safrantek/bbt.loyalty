using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum CustomerTypeEnum
    {
        [Description("Gerçek")]
        Real = 1,
        [Description("Tüzel")]
        Corporate = 2,
        [Description("Ortak")]
        Partnership = 3,
        [Description("Reşit Olmayan")]
        Underage = 4,
        [Description("Adi Ortaklık")]
        OrdinaryPartnership = 5

    }
}
