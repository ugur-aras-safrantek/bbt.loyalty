using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum TargetViewTypeEnum
    {
        [Description("Progress Bar")]
        ProgressBar = 1,
        [Description("Bilgi")]
        Information = 2,
        [Description("Görüntülenmeyecek")]
        Invisible = 3,
    }
}
