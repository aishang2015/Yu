using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Yu.Core.Constants;
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

                    // 初始化实体数据表
                    InitEntityData(dbContext);

                    // 初始化成员和角色
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<BaseIdentityUser>>();
                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<BaseIdentityRole>>();
                    InitMember(userManager, roleManager);

                    // 初始化其他数据
                    InitOtherData(dbContext);                   

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

        // 初始化实体数据表
        private static void InitEntityData<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            // 所有实体的类型
            var types = EntityTypeFinder.FindEntityTypes();

            foreach (var type in types)
            {
                // 数据库上下文
                var attribute = type.GetCustomAttributes(typeof(BelongToAttribute), false).FirstOrDefault();
                var dbcontext = attribute != null ? ((BelongToAttribute)attribute).DbContextType.Name : string.Empty;

                // 表和表名
                var table = type.Name;
                attribute = type.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                var tableDescription = attribute != null ? ((DescriptionAttribute)attribute).Description : string.Empty;

                foreach (var prop in type.GetProperties())
                {
                    if (prop.PropertyType.IsPublic)
                    {
                        // 字段和字段名
                        var field = prop.Name;
                        attribute = prop.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault();
                        var fieldDescription = attribute != null ? ((DescriptionAttribute)attribute).Description : string.Empty;

                        dbContext.Set<Entity>().Add(new Entity
                        {
                            DbContext = dbcontext,
                            Table = table,
                            TableDescribe = tableDescription,
                            Field = field,
                            FieldDescribe = fieldDescription
                        });
                    }
                }
            }

            // 保存数据
            dbContext.SaveChanges();

        }

        // 初始化成员
        private static void InitMember(UserManager<BaseIdentityUser> userManager, RoleManager<BaseIdentityRole> roleManager)
        {
            //for (int i = 0; i < 10; i++)
            //{
            //    var user = new BaseIdentityUser
            //    {
            //        UserName = $"admin{i}",
            //        NormalizedUserName = $"ADMIN{i}",
            //        SecurityStamp = Guid.NewGuid().ToString()
            //    };
            //    user.PasswordHash = new PasswordHasher<BaseIdentityUser>().HashPassword(user, CommonConstants.Password);
            //    dbContext.Set<BaseIdentityUser>().Add(user);
            //}
            //dbContext.SaveChanges();     

            roleManager.CreateAsync(new BaseIdentityRole
            {
                Name = "系统管理员",
                Describe = "拥有系统的全部操作权限",
            }).Wait();
            for (int i = 0; i < 10; i++)
            {
                var user = new BaseIdentityUser
                {
                    UserName = $"admin{i}",
                    NormalizedUserName = $"ADMIN{i}",
                    Roles = "系统管理员"
                };
                userManager.CreateAsync(user, CommonConstants.Password).Wait();
                userManager.AddToRoleAsync(user, "系统管理员").Wait();
            }

            


        }

        // 初始化其他数据
        private static void InitOtherData<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'权限管理', 1, N'rightmanage', N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'ad592b36-dd5b-447b-fa6d-08d7072f517b', N'用户管理', 1, N'usermanage', N'/right/user')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b', N'角色管理', 1, N'rolemanage', N'/right/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'62663b74-a5d0-43ed-fa70-08d7072f517b', N'组织管理', 1, N'groupmanage', N'/right/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'5b859edf-eac7-46a7-fa71-08d7072f517b', N'页面元素管理', 1, N'elementmanage', N'/right/menu')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'b08a4c30-d492-4537-fa72-08d7072f517b', N'api数据管理', 1, N'apimanage', N'/right/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'e68887e2-1f67-4ee3-fa73-08d7072f517b', N'规则管理', 1, N'rulemanage', N'/right/rule')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[Element] ([Id], [Name], [ElementType], [Identification], [Route]) VALUES (N'515f8064-e91c-41e2-fa74-08d7072f517b', N'实体数据管理', 1, N'entitymanage', N'/right/entity')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'05e877a1-d897-4a68-6d14-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'b66e1bef-da5d-4ad1-6d15-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'ad592b36-dd5b-447b-fa6d-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'2b83f45d-0b18-44fb-6d16-08d7072f517f', N'ad592b36-dd5b-447b-fa6d-08d7072f517b', N'ad592b36-dd5b-447b-fa6d-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'cdbaf940-e8ae-4fe9-6d17-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'80f5ef02-0277-4d95-6d18-08d7072f517f', N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b', N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'd059c25b-ba9f-4ac0-6d1c-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'62663b74-a5d0-43ed-fa70-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'29fdbb81-ed7d-4f22-6d1d-08d7072f517f', N'62663b74-a5d0-43ed-fa70-08d7072f517b', N'62663b74-a5d0-43ed-fa70-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'39d441b8-9a2b-47dd-6d1e-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'5b859edf-eac7-46a7-fa71-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'c8ae766b-e077-4e7b-6d1f-08d7072f517f', N'5b859edf-eac7-46a7-fa71-08d7072f517b', N'5b859edf-eac7-46a7-fa71-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'f6a9a41f-59d9-4b07-6d20-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'b08a4c30-d492-4537-fa72-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'4274a4dd-9660-4906-6d21-08d7072f517f', N'b08a4c30-d492-4537-fa72-08d7072f517b', N'b08a4c30-d492-4537-fa72-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'f37856a6-6221-4630-6d22-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'e68887e2-1f67-4ee3-fa73-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'41c5a517-75a4-4834-6d23-08d7072f517f', N'e68887e2-1f67-4ee3-fa73-08d7072f517b', N'e68887e2-1f67-4ee3-fa73-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'69253c30-9f02-4fc9-6d24-08d7072f517f', N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'515f8064-e91c-41e2-fa74-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ElementTree] ([Id], [Ancestor], [Descendant], [Length]) VALUES (N'487f5dd0-65a0-4365-6d25-08d7072f517f', N'515f8064-e91c-41e2-fa74-08d7072f517b', N'515f8064-e91c-41e2-fa74-08d7072f517b', 0)");

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
