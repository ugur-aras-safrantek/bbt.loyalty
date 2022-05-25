using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;

namespace Bbt.Campaign.Services.Services.Cache
{
    public interface ICacheServis
    {
        Task<bool> ClearCache();
        Task<bool> RemoveAsync(string cacheKey);
        Task<bool> RemoveByPatternAsync(string cacheKey);
    }
}
