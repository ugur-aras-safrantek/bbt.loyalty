using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum TargetSourceEnum
    {
        [Description("Akış")]
        Flow = 1,
        [Description("Sorgu")]
        Query = 2,
    }
}
