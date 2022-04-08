using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Core.Cron
{
    [Flags]
    internal enum CronExpressionFlag : byte
    {
        DayOfMonthLast = 0b00001,
        DayOfWeekLast = 0b00010,
        Interval = 0b00100,
        NearestWeekday = 0b01000,
        NthDayOfWeek = 0b10000
    }
}
