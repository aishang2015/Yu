using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Redis
{
    public static class StackExchangeRedisServiceCollectionExtension
    {
        /// <summary>
        /// 手动模式通过helper来访问缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddStatckExchangeRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOption>(configuration.GetSection("RedisOption"));

            services.AddSingleton<RedisConnectionHelper>();
        }

        /// <summary>
        /// 通过AddDistributedRedisCache扩展调用redis
        /// </summary>
        /// <param name="services"></param>
        public static void AddDistributedRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisOption>(configuration.GetSection("RedisOption"));

            var option = services.BuildServiceProvider().GetRequiredService<IOptions<RedisOption>>().Value;

            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = option.Name;
                options.Configuration = option.RedisConnectionString;
            });
        }
    }
}
