using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Shared.Extentions
{
    public static class Utilities
    {
        public static string GetTerm()
        {
          return DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2");
        }
    }
}
