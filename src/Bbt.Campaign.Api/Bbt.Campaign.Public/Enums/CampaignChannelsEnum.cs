using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum CampaignChannelsEnum
    {
        [Description("Tümü")]
        All = 1,
        [Description("Batch")]
        Batch = 2,        
        [Description("Bayi")]
        Dealer = 3,        
        [Description("Diğer")]
        Other = 4,
        [Description("İnternet")]
        Internet = 5,
        [Description("Ptt")]
        Ptt = 6,
        [Description("Remote")]
        Remote = 7,
        [Description("Sms")]
        Sms = 8,
        [Description("Şube")]
        Branch = 9,
        [Description("Tablet")]
        Tablet = 10,
        [Description("Web")]
        Web = 11,
        [Description("Web Bayi")]
        WebDealer = 12,
        [Description("Web Mevduat")]
        WebDeposit = 13,
    }
}
