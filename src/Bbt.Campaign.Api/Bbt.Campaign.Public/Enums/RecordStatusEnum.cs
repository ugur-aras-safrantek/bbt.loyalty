using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum RecordStatusEnum
    {
        [Description("Yeni")]
        New = 10,
        [Description("Taslak")]
        Draft = 20,
        [Description("Onayda Bekliyor")]
        SentToApprove = 30,
        [Description("Tarihçe")]
        History = 40,
        [Description("Onaylandı")]
        Approved = 100,
    }
}
