using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Public.Dtos;
using Bbt.Campaign.Services.Services.Remote;
using Bbt.Campaign.Shared.ServiceDependencies;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Bbt.Campaign.Services.Services.Cache
{
    public class CacheServis : ICacheServis, IScopedService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IRedisDatabaseProvider _redisDatabaseProvider;
        private readonly IRemoteService _remoteService;

        public CacheServis( IRedisDatabaseProvider redisDatabaseProvider, IMemoryCache memoryCache, IRemoteService remoteService)
        {
            _redisDatabaseProvider = redisDatabaseProvider;
            _memoryCache = memoryCache;
            _remoteService = remoteService;
        }

        public async Task<bool> ClearCache()
        {
            //PropertyInfo prop = _memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            //object innerCache = prop.GetValue(_memoryCache);
            //MethodInfo clearMethod = innerCache.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            //clearMethod.Invoke(innerCache, null);
            var resp = await _remoteService.CleanCache();
            await _redisDatabaseProvider.FlushDatabase();

            return resp && true;
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
