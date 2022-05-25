using Bbt.Campaign.Api.Base;
using Bbt.Campaign.Services.Services.Cache;
using Microsoft.AspNetCore.Mvc;

namespace Bbt.Campaign.Api.Controllers
{
    public class CacheController : BaseController<ApproveController>
    {
        private readonly ICacheServis _cacheService;

        public CacheController(ICacheServis cacheService)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// Clears all keys on the cache.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("clear-all")]
        public async Task<IActionResult> ClearCacheRedis()
        {
            var result = await _cacheService.ClearCache();
            return Ok(result);
        }

        /// <summary>
        /// Removes the cache key from redis
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns></returns>
        [HttpGet]
        [Route("remove-key")]
        public async Task<IActionResult> RemoveAsync(string cacheKey)
        {
            var result = await _cacheService.RemoveAsync(cacheKey);
            return Ok(result);
        }

        /// <summary>
        /// Removes the cache key pattern from redis
        /// </summary>
        /// <param name="cacheKey">cacheKey</param>
        /// <returns></returns>
        [HttpGet]
        [Route("remove-key-by-pattern")]
        public async Task<IActionResult> RemoveByPatternAsync(string cacheKey)
        {
            var result = await _cacheService.RemoveByPatternAsync(cacheKey);
            return Ok(result);
        }
    }
}
