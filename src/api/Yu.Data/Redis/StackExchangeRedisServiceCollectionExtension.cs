using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Redis
{
    public static class StackExchangeRedisServiceCollectionExtension
    {
        public static void AddStatckExchangeRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOption>(configuration.GetSection("RedisOption"));

            services.AddSingleton<RedisConnectionHelper>();
        }
    }
}
