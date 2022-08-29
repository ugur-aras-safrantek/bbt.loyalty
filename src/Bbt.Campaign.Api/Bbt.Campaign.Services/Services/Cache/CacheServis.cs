using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Bbt.Campaign.Services.Services.Cache
{
    public class CacheServis : ICacheServis, IScopedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;

        public CacheServis( IRedisDatabaseProvider redisDatabaseProvider, IMemoryCache memoryCache)
        {
            _redisDatabaseProvider = redisDatabaseProvider;
            _memoryCache = memoryCache;
        }

        public async Task<bool> ClearCache()
        {
            //PropertyInfo prop = _memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            //object innerCache = prop.GetValue(_memoryCache);
            //MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            //clearMethod.Invoke(innerCache, null);

            await _redisDatabaseProvider.FlushDatabase();

            return true;
        }

        public Task<bool> RemoveAsync(string cacheKey) 
        { 
            return _redisDatabaseProvider.RemoveAsync(cacheKey);
        }

        public Task<bool> RemoveByPatternAsync(string cacheKey)
        {
            return _redisDatabaseProvider.RemoveByPattern(cacheKey);
        }

        //public bool Exists(string key)
        //{
        //    return _memoryCache.KeyExists(key);
        //}

        //public void Save(string key, string value)
        //{
        //    var ts = TimeSpan.FromMinutes(_settings.CacheTimeout);
        //    _cache.StringSet(key, value, ts);
        //}

        //public string Get(string key)
        //{
        //    return _cache.StringGet(key);
        //}

        //public void Remove(string key)
        //{
        //    _cache.KeyDelete(key);
        //}
    }
}
