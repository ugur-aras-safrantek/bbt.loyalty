using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Core.Cron
{
    internal static class DateTimeHelper
    {
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        public static DateTimeOffset FloorToSeconds(DateTimeOffset dateTimeOffset) => dateTimeOffset.AddTicks(-GetExtraTicks(dateTimeOffset.Ticks));

        public static bool IsRound(DateTimeOffset dateTimeOffset) => GetExtraTicks(dateTimeOffset.Ticks) == 0;

        private static long GetExtraTicks(long ticks) => ticks % OneSecond.Ticks;
    }
}
