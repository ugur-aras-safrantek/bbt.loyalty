
using System.ComponentModel;


namespace Bbt.Campaign.Public.Enums
{
    public enum TargetOperationsEnum
    {
        [Description("VE")]
        And = 1,
        [Description("VEYA")]
        Or = 2,
        [Description("KESİŞİM")]
        intersection = 3,
        [Description("FARK")]
        Difference = 4,

    }
}
