using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bbt.Campaign.EntityFrameworkCore.Redis
{
    public interface IRedisDatabaseProvider
    {
        Task FlushDatabase();
        Task SetAsync(string cacheKey, string value);
        Task<string> GetAsync(string cacheKey);
        Task<bool> RemoveAsync(string cacheKey);
        Task SetWithMinuteAsync(string cacheKey, string value, int minutes);
        Task<bool> RemoveByPattern(string pattern);
    }
}
