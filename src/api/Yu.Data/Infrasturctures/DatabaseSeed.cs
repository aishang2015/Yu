using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Yu.Core.Mvc;
using Yu.Data.Entities.Right;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 数据库数据初始化
    /// </summary>
    public static class DatabaseSeed
    {
        /// <summary>
        /// 初始化认证数据库数据
        /// </summary>
        /// <param name="app"></param>
        public static void SeedIdentityDbData<TDbContext>(this IApplicationBuilder app, Action<TDbContext> dataSeed = null) where TDbContext : BaseIdentityDbContext
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // 检查数据库状态
                if (serviceScope.ServiceProvider.GetRequiredService<TDbContext>().Database.EnsureCreated())
                {
                    // 初始化数据
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

                    // 初始化API数据
                    InitApi(dbContext, app);

                    // 初始化成员
                    InitMember(dbContext);

                    // 初始化角色

                    // 自定义初始方法
                    dataSeed?.Invoke(dbContext);
                }
                else
                {
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

                    // 更新API数据
                    UpdateApi(dbContext, app);

                }
            }
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="app"></param>
        public static void SeedDbData<TDbContext>(this IApplicationBuilder app, Action<TDbContext> dataSeed = null) where TDbContext : DbContext
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                // 检查数据库状态
                if (serviceScope.ServiceProvider.GetRequiredService<TDbContext>().Database.EnsureCreated())
                {
                    // 初始化数据
                    var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                    
                    // 自定义初始方法
                    dataSeed?.Invoke(dbContext);
                }
            }
        }

        // 初始化API数据
        private static void InitApi<TDbContext>(TDbContext dbContext, IApplicationBuilder app) where TDbContext : DbContext
        {
            var result = app.GetApiInfos();
            var apis = result.Select(api => new Api
            {
                Name = api.Item1,
                Controller = api.Item2,
                Type = api.Item3,
                Address = api.Item4
            });
            dbContext.Set<Api>().AddRange(apis);
            dbContext.SaveChanges();
        }

        // 初始化成员
        private static void InitMember<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            for (int i = 0; i < 10; i++)
            {
                var user = new BaseIdentityUser
                {
                    UserName = $"admin{i}",
                    NormalizedUserName = $"ADMIN{i}",
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                user.PasswordHash = new PasswordHasher<BaseIdentityUser>().HashPassword(user, "P@ssword1");
                dbContext.Set<BaseIdentityUser>().Add(user);
                dbContext.SaveChanges();
            }
        }

        // 更新API数据
        private static void UpdateApi<TDbContext>(TDbContext dbContext, IApplicationBuilder app) where TDbContext : DbContext
        {
            var result = app.GetApiInfos();
            var apis = result.Select(api => new Api
            {
                Name = api.Item1,
                Controller = api.Item2,
                Type = api.Item3,
                Address = api.Item4
            });
            var existApi = dbContext.Set<Api>().ToList();
            var removeApis = existApi.Except(apis, new ApiEquality());
            var addApis = apis.Except(existApi, new ApiEquality());
            dbContext.Set<Api>().RemoveRange(removeApis);
            dbContext.Set<Api>().AddRange(addApis);
            dbContext.SaveChanges();
        }

        private class ApiEquality : IEqualityComparer<Api>
        {
            public bool Equals(Api x, Api y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(Api obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                else
                {
                    return obj.ToString().GetHashCode();
                }
            }
        }
    }
}
