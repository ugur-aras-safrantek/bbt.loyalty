using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Public.Enums
{
    public enum CustomerFormTypesEnum
    {
        [Description("Görüntüleme")]
        View = 1,
        [Description("Kampanya Katılım")]
        Join = 2,
    }
}
