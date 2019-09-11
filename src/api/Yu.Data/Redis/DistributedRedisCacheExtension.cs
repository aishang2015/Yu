using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yu.Data.Redis
{
    public static class DistributedRedisCacheExtension
    {
        
        /// <summary>
        /// 保存缓存对象
        /// </summary>
        public static void SetObject<T>(this IDistributedCache cache, string key, T o, DistributedCacheEntryOptions options = null)
        {
            var result = o is string ? o.ToString() : JsonConvert.SerializeObject(o);
            cache.SetString(key, result, options);
        }

        /// <summary>
        /// 保存缓存对象
        /// </summary>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, string key, T o, DistributedCacheEntryOptions options = null)
        {
            var result = o is string ? o.ToString() : JsonConvert.SerializeObject(o);
            await cache.SetStringAsync(key, result, options);
        }

        /// <summary>
        /// 取得缓存对象
        /// </summary>
        public static T GetObject<T>(this IDistributedCache cache, string key)
        {
            var result = cache.GetString(key);
            return JsonConvert.DeserializeObject<T>(result);
        }

        /// <summary>
        /// 取得缓存对象
        /// </summary>
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, string key)
        {
            var result = await cache.GetStringAsync(key);
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
