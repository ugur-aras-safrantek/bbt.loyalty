using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum AchievementTypeEnum
    {
        [Description("Mevduat")]
        Deposit = 1,
        [Description("Kredi")]
        Credit = 2,
        [Description("Cashback")]
        Cashback = 3,
    }
}
