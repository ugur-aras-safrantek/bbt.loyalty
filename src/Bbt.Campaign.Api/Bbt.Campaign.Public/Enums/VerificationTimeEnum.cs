
using System.ComponentModel;


namespace Bbt.Campaign.Public.Enums
{
    public enum VerificationTimeEnum
    {

        [Description("İlk Kontrol Edildiğinde")]
        InFirstControl = 1,
        [Description("Her Kontrol Edildiğinde")]
        InEveryControl = 2,
    }
}
