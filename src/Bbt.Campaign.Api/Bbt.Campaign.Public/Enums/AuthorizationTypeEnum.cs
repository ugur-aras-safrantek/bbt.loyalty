using System.ComponentModel;


namespace Bbt.Campaign.Public.Enums
{
    public enum AuthorizationTypeEnum
    {
        [Description("Yeni Kayıt")]
        Insert = 1,
        [Description("Kayıt Güncelleme")]
        Update = 2,
        [Description("İzleme")]
        View = 3,
        [Description("Onay")]
        Approve = 4
    }
}
