using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum TopLimitType
    {
        [Description("Tutar")]
        Amount = 1, 
        [Description("Oran")]
        Rate = 2,
    }
}
