using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum StatusEnum
    {
        [Description("Taslak")]
        Draft = 1,
        [Description("Onaya Gönderildi")]
        SentToApprove = 2,
        [Description("Güncelleniyor")]
        Updating = 3,
        [Description("Tarihçe")]
        History = 4,
        [Description("Onaylandı")]
        Approved = 10,
    }
}
