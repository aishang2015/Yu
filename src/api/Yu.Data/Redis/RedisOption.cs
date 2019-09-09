using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Redis
{
    public class RedisOption
    {
        // redishelper用
        public string RedisKey { get; set; }

        public string RedisExchangeHosts { get; set; }

        // idistributedcache用连接串
        public string Name { get; set; }

        public string RedisConnectionString { get; set; }

    }
}
