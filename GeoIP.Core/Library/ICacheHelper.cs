using GeoIP.Core.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoIP.Core.Library
{
    public interface ICacheHelper
    {
        TItem TryGetOrCreate<TItem>(string key, TimeSpan timeSpan, Func<TItem> factory,
       RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<TItem> TryGetOrCreateAsync<TItem>(string key, TimeSpan? timeSpan, Func<Task<TItem>> factory,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<bool> StringFireAndForgetSetAsync(string key, RedisValue value, TimeSpan? timeSpan,
                RedisDataBase redisDataBase = RedisDataBase.Shared);

        bool StringSet(string key, RedisValue value, TimeSpan? timeSpan,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        string StringGet(string key, RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<long> StringIncrementAsync(string key, TimeSpan? timeSpan,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        bool KeyExists(string key, RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<TItem> StringGetAsync<TItem>(string key, RedisDataBase redisDataBase = RedisDataBase.Shared);
        List<TItem> GetCacheList<TItem>(IEnumerable<string> keys, RedisDataBase redisDataBase = RedisDataBase.Shared);

        bool SetCacheList<TItem>(Dictionary<string, TItem> dataList, TimeSpan? timeSpan,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        bool InsertList<TItem>(string key, TimeSpan? timeSpan, IEnumerable<TItem> list,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        List<TItem> GetListByRange<TItem>(string key, long startIndex, long endIndex,
            RedisDataBase redisDataBase = RedisDataBase.Shared);

        long GetListLength(string key, RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<bool> KeyDeleteAsync(string key, RedisDataBase redisDataBase = RedisDataBase.Shared);
        Task<bool> SetNxAsync(string key, string value, TimeSpan timeSpan, RedisDataBase redisDataBase = RedisDataBase.Shared);

        Task<bool> TryCreateLockAsync(RedisKey lockKey, RedisValue lockValue, TimeSpan? expiry, RedisDataBase redisDataBase = RedisDataBase.Shared);
        Task<bool> UnLockAsync(RedisKey lockKey, RedisValue lockValue, RedisDataBase redisDataBase = RedisDataBase.Shared);
    }
}
