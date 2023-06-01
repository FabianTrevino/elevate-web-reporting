using System;
using System.Threading.Tasks;

namespace DM.WR.GraphQlClient
{
    public interface ICacheWrapper
    {
        T GetFromCache<T>(string key, Func<T> missedCacheCall);

        Task<T> GetFromCacheAsync<T>(string key, Func<Task<T>> missedCacheCall);

        T GetFromCache<T>(string key, Func<T> missedCacheCall, TimeSpan timeToLive);

        Task<T> GetFromCacheAsync<T>(string key, Func<Task<T>> missedCacheCall, TimeSpan timeToLive);

        void SetCache(string key, object obj);

        void InvalidateCache(string key);
    }
}