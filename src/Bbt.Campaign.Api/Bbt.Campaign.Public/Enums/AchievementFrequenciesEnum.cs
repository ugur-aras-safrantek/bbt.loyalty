using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum AchievementFrequenciesEnum
    {
        [Description("Haftalık")]
        Weekly = 1,
        [Description("Aylık")]
        Monthly = 2,
        [Description("Yıllık")]
        Yearly = 3,
    }
}
