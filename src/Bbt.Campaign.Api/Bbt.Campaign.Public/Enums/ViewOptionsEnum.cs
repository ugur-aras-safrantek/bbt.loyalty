using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum ViewOptionsEnum
    {
        [Description("Sürekli Kampanyalar")]
        ConstantCampaign = 1,
        [Description("Dönemsel Kampanyalar”")]
        PeriodicalCampaign = 2,
        [Description("Genel Kampanyalar")]
        GeneralCampaign = 3,
        [Description("Görüntülenmeyecek")]
        InvisibleCampaign = 4,
    }
}
