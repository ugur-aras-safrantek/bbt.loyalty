using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum TriggerTimeEnum
    {
        [Description("Hedefe Ulaşıldığı Anda")]
        OnComplete = 1,
        [Description("Tamamlandıktan Sonra")]
        AfterComplete = 2,
    }
}
