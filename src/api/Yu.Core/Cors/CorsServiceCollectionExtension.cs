using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Yu.Core.Cors
{
    /// <summary>
    /// 跨域设置扩展
    /// </summary>
    public static class CorsServiceCollectionExtension
    {

        // 跨域策略名
        private static string _policyName;
        
        /// <summary>
        /// 添加跨域配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">配置工具</param>
        public static void AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            // 读取配置文件
            var policyName = configuration["CorsPolicyOptions:PolicyName"];
            _policyName = policyName;
            var origin = configuration["CorsPolicyOptions:Origin"];
            var method = configuration["CorsPolicyOptions:Method"];
            var header = configuration["CorsPolicyOptions:Header"];
            var exposedHeaders = configuration["CorsPolicyOptions:ExposedHeaders"];

            // 生成CorsPolicyBuilder
            Action<CorsPolicyBuilder> configurePolicy = builder =>
            {
                // 设置Origin
                if (string.IsNullOrEmpty(origin))
                {
                    builder.AllowAnyOrigin();
                }
                else
                {
                    builder.WithOrigins(origin);
                }

                // 设置Mehtod
                if (string.IsNullOrEmpty(method))
                {
                    builder.AllowAnyMethod();
                }
                else
                {
                    builder.WithMethods(method);
                }

                // 设置Header
                if (string.IsNullOrEmpty(header))
                {
                    builder.AllowAnyHeader();
                }
                else
                {
                    builder.WithHeaders(header);
                }

                // 设置ExposedHeaders
                if (!string.IsNullOrEmpty(exposedHeaders))
                {
                    builder.WithExposedHeaders();
                }

            };

            // 设置
            services.AddCors(ops =>
            {
                ops.AddPolicy(policyName, configurePolicy);
            });
        }

        /// <summary>
        /// 使用跨域策略
        /// </summary>
        /// <param name="app">app</param>
        public static void UseCustomCors(this IApplicationBuilder app)
        {
            app.UseCors(_policyName);
        }



    }
}
