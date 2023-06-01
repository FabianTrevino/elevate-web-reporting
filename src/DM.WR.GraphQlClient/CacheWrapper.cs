using DM.WR.Models.Config;
using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace DM.WR.GraphQlClient
{
    public class CacheWrapper : ICacheWrapper
    {
        private readonly ObjectCache _cache = MemoryCache.Default;

        public T GetFromCache<T>(string key, Func<T> missedCacheCall)
        {
            return GetFromCache(key, missedCacheCall, TimeSpan.FromMinutes(ConfigSettings.GraphQlResponseCacheTime));
        }

        public async Task<T> GetFromCacheAsync<T>(string key, Func<Task<T>> missedCacheCall)
        {
            return await GetFromCacheAsync<T>(key, missedCacheCall, TimeSpan.FromMinutes(ConfigSettings.GraphQlResponseCacheTime));
        }

        public T GetFromCache<T>(string key, Func<T> missedCacheCall, TimeSpan timeToLive)
        {
            var obj = _cache.Get(key);

            if (obj == null)
            {
                obj = missedCacheCall();
                if (obj != null)
                {
                    _cache.Set(key, obj, DateTimeOffset.Now.Add(timeToLive));
                }
            }

            return (T)obj;
        }
        
        public async Task<T> GetFromCacheAsync<T>(string key, Func<Task<T>> missedCacheCall, TimeSpan timeToLive)
        {
            var obj = _cache.Get(key);

            if (obj == null)
            {
                obj = await missedCacheCall();
                if (obj != null)
                {
                    _cache.Set(key, obj, DateTimeOffset.Now.Add(timeToLive));
                }
            }

            return (T)obj;
        }

        public void SetCache(string key, object obj)
        {
            _cache.Set(key, obj, DateTimeOffset.Now.Add(TimeSpan.FromMinutes(ConfigSettings.GraphQlResponseCacheTime)));
        }

        public void InvalidateCache(string key)
        {
            _cache.Remove(key);
        }
    }
}