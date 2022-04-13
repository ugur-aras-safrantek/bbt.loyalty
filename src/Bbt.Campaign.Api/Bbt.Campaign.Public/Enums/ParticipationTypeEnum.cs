
using System.ComponentModel;

namespace Bbt.Campaign.Public.Enums
{
    public enum ParticipationTypeEnum
    {
        [Description("Otomatik Katılım")]
        AutomaticParticipation = 1,
        [Description("Müşteri Seçimi")]
        CustomerSelected = 2,
        [Description("Operatör Seçimi")]
        OperatorSelected = 3,
    }
}
