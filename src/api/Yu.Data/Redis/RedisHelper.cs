using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yu.Data.Redis
{
    public partial class RedisHelper
    {
        private int DbNum { get; } = 0;

        private readonly RedisConnectionHelper _connectionHelper;

        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHelper(RedisConnectionHelper connectionHelper, IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionHelper = connectionHelper;
            _connectionMultiplexer = connectionMultiplexer;
        }

        private string AddSysCustomKey(string oldKey)
        {
            var prefix = _connectionHelper.SysCustomKey;
            return prefix + oldKey;
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _connectionMultiplexer.GetDatabase(DbNum);
            return func(database);
        }

        private string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private T ConvertObj<T>(RedisValue value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }
    }
}
