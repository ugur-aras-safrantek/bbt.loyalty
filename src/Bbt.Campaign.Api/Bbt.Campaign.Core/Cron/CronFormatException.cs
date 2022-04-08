using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.Core.Cron
{
#if !NETSTANDARD1_0
    [Serializable]
#endif
    public class CronFormatException : FormatException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CronFormatException"/> class with
        /// the given message.
        /// </summary>
        public CronFormatException(string message) : base(message)
        {
        }

        internal CronFormatException(CronField field, string message) : this($"{field}: {message}")
        {
        }
    }
}
