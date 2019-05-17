using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 数据库数据初始化
    /// </summary>
    public static class DatabaseSeed
    {
        /// <summary>
        /// 启动时初始化数据
        /// </summary>
        /// <param name="app"></param>
        public static void SeedDbData(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // 检查数据库状态
                if (serviceScope.ServiceProvider.GetRequiredService<BaseDbContext>().Database.EnsureCreated())
                {
                    // 初始化数据
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<BaseDbContext>();
                }
            }
        }
    }
}
