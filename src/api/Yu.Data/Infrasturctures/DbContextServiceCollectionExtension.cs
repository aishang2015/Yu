using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yu.Data.Infrasturctures.Mvc;
using Yu.Data.Infrasturctures.MySql;
using Yu.Data.Infrasturctures.Pemission;
using Yu.Data.Infrasturctures.PostgreSql;
using Yu.Data.Infrasturctures.SqlLite;
using Yu.Data.Infrasturctures.SqlServer;
using Yu.Data.Repositories;

namespace Yu.Data.Infrasturctures
{
    public static class DbContextServiceCollectionExtension
    {
        /// <summary>
        /// 添加认证数据库上下文服务
        /// </summary>
        /// <typeparam name="T">用户和角色实体的主键类型</typeparam>
        /// <param name="services">服务集</param>
        /// <param name="connectionString">数据库连接串</param>
        /// <param name="databaseType">数据库类型</param>
        /// <param name="setupAction">用户认证选项</param>
        public static void AddIdentityDbContext<TDbContext, TUser, TRole, Tkey>(this IServiceCollection services, string connectionString, DatabaseType databaseType,
            Action<IdentityOptions> setupAction = null)
            where TDbContext : IdentityDbContext<TUser, TRole, Tkey>
            where TUser : IdentityUser<Tkey>
            where TRole : IdentityRole<Tkey>
            where Tkey : IEquatable<Tkey>
        {
            services.AddDbContext<TDbContext>((provider, builder) =>
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
                    case DatabaseType.SqlServer:
                        SqlServerBuilder.UseSqlServer(builder, connectionString, sqlServerDbContextOptionsBuilder => { });
                        break;
                }
            })
            .AddIdentity<TUser, TRole>(setupAction)     // 使用user和role 进行认证
            .AddEntityFrameworkStores<TDbContext>()       // 使用dbcontext存储
            .AddDefaultTokenProviders();    // 添加默认token生成工具，用其生成的token用来进行密码重置。

            services.AddApiAuthorization(); // 添加api认证Handler

            services.AddScoped<IPermissionCacheService, PermissionCacheService>(); // 权限缓存服务

            services.AddRepositories<TDbContext>(); // 批量注入数据仓储
        }


        public static void AddCommonDbContext<TDbContext>(this IServiceCollection services, string connectionString, DatabaseType databaseType)
            where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>((provider, builder) =>
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
                    case DatabaseType.SqlServer:
                        SqlServerBuilder.UseSqlServer(builder, connectionString, sqlServerDbContextOptionsBuilder => { });
                        break;
                }
            });
        }
    }
}
