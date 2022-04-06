using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum ProgramTypeEnum
    {
        [Description("Sadakat")]
        Loyalty = 1,
        [Description("Kampanya")]
        Campaign = 2,
        [Description("Kazanım")]
        Achievement = 3,
    }
}
