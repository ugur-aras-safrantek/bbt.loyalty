using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;

namespace Bbt.Campaign.Services.Services.Cache
{
    public interface ICacheServis
    {
        Task<BaseResponse<RedisClearDto>> ClearCacheRedis();
    }
}
