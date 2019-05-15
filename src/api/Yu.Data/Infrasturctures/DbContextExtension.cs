using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yu.Data.Infrasturctures.MySql;
using Yu.Data.Infrasturctures.PostgreSql;
using Yu.Data.Infrasturctures.SqlLite;
using Yu.Data.Infrasturctures.SqlServer;

namespace Yu.Data.Infrasturctures
{
    public static class DbContextExtension
    {
        /// <summary>
        /// 添加认证数据库上下文服务
        /// </summary>
        /// <typeparam name="T">用户和角色实体的主键类型</typeparam>
        /// <param name="services">服务集</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="setupAction">用户认证选项</param>
        public static void AddIdentityDbContext(this IServiceCollection services, string connectionString, DatabaseType databaseType,
            Action<IdentityOptions> setupAction = null)
        {
            services.AddDbContext<BaseDbContext>((provider, builder) =>
            {
                // 微软efcore支持的数据库提供程序
                // https://docs.microsoft.com/zh-cn/ef/core/providers/
                switch (databaseType)
                {
                    case DatabaseType.MySql:
                        MySqlBuilder.UseMySql(builder, connectionString, mySqlDbContextOptionsBuilder => { });
                        break;
                    case DatabaseType.PostgreSql:
                        PostgreSqlBuilder.UsePostgreSql(builder, connectionString, npgsqlDbContextOptionsBuilder => { });
                        break;
                    case DatabaseType.SqlLite:
                        SqlLiteBuilder.UseSqlLite(builder, connectionString, sqliteDbContextOptionsBuilder => { });
                        break;
                    case DatabaseType.SqlServe:
                        SqlLiteBuilder.UseSqlLite(builder, connectionString, sqliteDbContextOptionsBuilder => { });
                        break;
                }
            })
            .AddIdentity<BaseUser<Guid>,BaseRole<Guid>>(setupAction)     // 使用user和role 进行认证
            .AddEntityFrameworkStores<BaseDbContext>()       // 使用dbcontext存储
            .AddDefaultTokenProviders();    // 添加默认token生成工具，用其生成的token用来进行密码重置。
        }
    }
}
