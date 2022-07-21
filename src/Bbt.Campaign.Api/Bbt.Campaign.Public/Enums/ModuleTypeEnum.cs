using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum ModuleTypeEnum
    {
        [Description("Kampanya")]
        Campaign = 1,
        [Description("Çatı Limit")]
        TopLimit = 2,
        [Description("Hedef")]
        Target = 3,
        [Description("Rapor")]
        Report = 4,
        [Description("Vkn/Tckn")]
        Identity = 5
    }
}
