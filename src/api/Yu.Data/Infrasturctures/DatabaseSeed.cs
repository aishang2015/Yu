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
                    //InitApi(dbContext, app);

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
            // 所有标注DataRuleManage特性的实体的类型
            var types = EntityTypeFinder.FindDataRuleManageEntityTypes();

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
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'9807863c-b234-4452-7d57-08d708fe7bef',N'账户管理-用户名密码登陆',N'Account',N'POST',N'/api/Account/Login')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'47317471-a6d6-4c6f-7d58-08d708fe7bef',N'账户管理-刷新token',N'Account',N'POST',N'/api/Account/RefreshToken')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'8b02d6f4-c6e7-44ea-7d59-08d708fe7bef',N'账户管理-修改密码',N'Account',N'POST',N'/api/Account/ChangePwd')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'4768aa73-11a1-4016-7d5a-08d708fe7bef',N'验证码-获取验证码',N'Captcha',N'GET',N'/api/Captcha')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'b23935af-93a3-4eee-7d5b-08d708fe7bef',N'接口管理-取得全部Api数据',N'Api',N'GET',N'/api/allApi')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'ec48a0c4-ffed-43ad-7d5c-08d708fe7bef',N'接口管理-取得API数据',N'Api',N'GET',N'/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'308b4d8f-5db1-47e4-7d5d-08d708fe7bef',N'接口管理-添加API数据',N'Api',N'POST',N'/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'cb72e62b-60cb-4188-7d5e-08d708fe7bef',N'接口管理-更新api数据',N'Api',N'PUT',N'/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'edfd819d-2424-446f-7d5f-08d708fe7bef',N'接口管理-删除api数据',N'Api',N'DELETE',N'/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'fa75a6b9-743e-4961-7d60-08d708fe7bef',N'页面元素管理-取得页面元素',N'Element',N'GET',N'/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'3e76b4cf-0ff3-4b75-7d61-08d708fe7bef',N'页面元素管理-添加新元素',N'Element',N'POST',N'/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'ff573676-00ce-46cf-7d62-08d708fe7bef',N'页面元素管理-删除元素',N'Element',N'DELETE',N'/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'7d619e77-c7c4-446b-7d63-08d708fe7bef',N'页面元素管理-更新元素',N'Element',N'PUT',N'/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'7a90f207-ec2a-435b-7d64-08d708fe7bef',N'实体管理-取得实体数据(下拉框用)',N'Entity',N'GET',N'/api/entities')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'b4d9c97c-a17d-48e5-7d65-08d708fe7bef',N'实体管理-取得实体数据',N'Entity',N'GET',N'/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'3f047f21-0c05-4f78-7d66-08d708fe7bef',N'实体管理-添加实体数据',N'Entity',N'POST',N'/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'1cbfc955-a4a4-48a2-7d67-08d708fe7bef',N'实体管理-更新实体数据',N'Entity',N'PUT',N'/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'5b3acd28-4d57-46ce-7d68-08d708fe7bef',N'实体管理-删除实体数据',N'Entity',N'DELETE',N'/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'598f8a41-6857-4012-7d69-08d708fe7bef',N'组织结构管理-取得所有组织',N'Group',N'GET',N'/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'0273a51b-6a4e-4b77-7d6a-08d708fe7bef',N'组织结构管理-删除组织',N'Group',N'DELETE',N'/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'22e89a74-3e03-4f1d-7d6b-08d708fe7bef',N'组织结构管理-添加组织',N'Group',N'POST',N'/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'a97d11cb-a691-4567-7d6c-08d708fe7bef',N'组织结构管理-更新组织',N'Group',N'PUT',N'/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'b809dfc3-7af0-43c1-7d6d-08d708fe7bef',N'角色管理-取得全部角色名称',N'Role',N'GET',N'/api/roleNames')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'75e133dc-63ae-4027-7d6e-08d708fe7bef',N'角色管理-取得角色概要数据',N'Role',N'GET',N'/api/roleOutline')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'26b0f3a0-dd75-4e44-7d6f-08d708fe7bef',N'角色管理-取得角色详细数据',N'Role',N'GET',N'/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'0df18f48-730c-4398-7d70-08d708fe7bef',N'角色管理-删除角色数据',N'Role',N'DELETE',N'/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'6330e0bd-42fc-4ac6-7d71-08d708fe7bef',N'角色管理-添加角色',N'Role',N'POST',N'/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'90678f43-39c7-4ccf-7d72-08d708fe7bef',N'角色管理-更新角色',N'Role',N'PUT',N'/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'8488942f-ed09-4d55-7d73-08d708fe7bef',N'规则管理-查看所有规则组',N'Rule',N'GET',N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'a6b958f7-5d93-456c-7d74-08d708fe7bef',N'规则管理-查看规则组内容',N'Rule',N'GET',N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'34a24768-5d7b-4ead-7d75-08d708fe7bef',N'规则管理-删除规则组',N'Rule',N'DELETE',N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'09861d8a-2d36-4d6f-7d76-08d708fe7bef',N'规则管理-添加修改规则组',N'Rule',N'PUT',N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'5b343f0f-d587-4e10-7d77-08d708fe7bef',N'用户管理-取得用户概要情报',N'User',N'GET',N'/api/userOutline')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'3220f72a-a4ca-4326-7d78-08d708fe7bef',N'用户管理-取得用户数据',N'User',N'GET',N'/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'afb4c264-b57a-4936-7d79-08d708fe7bef',N'用户管理-添加新用户',N'User',N'POST',N'/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'40998129-255c-4edf-7d7a-08d708fe7bef',N'用户管理-更新用户数据',N'User',N'PUT',N'/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'7ec86556-becf-49f3-7d7b-08d708fe7bef',N'用户管理-删除用户数据',N'User',N'DELETE',N'/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Api]([Id],[Name],[Controller],[Type],[Address])VALUES(N'65f688c1-c679-4d38-7d7c-08d708fe7bef',N'用户管理-设置用户头像',N'User',N'POST',N'api/userAvatar')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'权限管理',1,N'rightmanage',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'用户管理',1,N'usermanage',N'/right/user')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'角色管理',1,N'rolemanage',N'/right/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'组织管理',1,N'groupmanage',N'/right/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'页面元素管理',1,N'elementmanage',N'/right/menu')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'b08a4c30-d492-4537-fa72-08d7072f517b',N'api数据管理',1,N'apimanage',N'/right/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'规则管理',1,N'rulemanage',N'/right/rule')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'515f8064-e91c-41e2-fa74-08d7072f517b',N'实体数据管理',1,N'entitymanage',N'/right/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'96469b51-7236-48c4-363c-08d709bf0b65',N'添加用户',2,N'adduserbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'c1be82cf-cf72-425f-363d-08d709bf0b65',N'查看用户',3,N'viewuserlink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'88877f44-13f8-41c5-363e-08d709bf0b65',N'编辑头像',3,N'editavatarlink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'b3440e7f-5aa1-424a-363f-08d709bf0b65',N'编辑信息',3,N'edituserlink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'3f70983f-1884-49ad-3640-08d709bf0b65',N'删除用户',3,N'deleteuserlink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'c27b893b-796d-4bbf-3641-08d709bf0b65',N'添加角色',2,N'addrolebtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'7c3e6882-22c3-45e7-3643-08d709bf0b65',N'修改角色',3,N'editrolelink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'f41535d8-03ec-4005-3644-08d709bf0b65',N'删除角色',3,N'deleterolelink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'5d542eef-5b8a-47b4-3645-08d709bf0b65',N'创建组织',2,N'addgroupbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',N'修改组织',2,N'editgroupbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'6f9ae434-29bc-43b6-3647-08d709bf0b65',N'删除组织',2,N'deletegroupbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'f3e3bc58-3808-429c-3648-08d709bf0b65',N'添加元素',2,N'addelementbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'0161d8ed-91de-48d2-3649-08d709bf0b65',N'修改元素',2,N'editelementbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'fbcb28e3-36b3-4227-364a-08d709bf0b65',N'删除元素',2,N'deleteelementbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'f9a8bd61-348e-4730-364b-08d709bf0b65',N'添加api',2,N'addapibtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'3d51d0dc-592d-4263-364c-08d709bf0b65',N'删除api',3,N'deleteapilink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'6ce2bc0b-951e-4818-364d-08d709bf0b65',N'修改api',3,N'editapilink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'f736892d-c4a7-4012-3651-08d709bf0b65',N'添加规则',2,N'addrulegroupbtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'29c3b92a-2252-42fc-3652-08d709bf0b65',N'编辑规则',3,N'editrulegroup',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'f72a8122-cde0-4e65-3653-08d709bf0b65',N'删除规则',3,N'deleterulegrouplink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'e6e3efe1-10f0-4078-3654-08d709bf0b65',N'添加数据',2,N'addentitybtn',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'a93f70c4-61b9-481f-3655-08d709bf0b65',N'修改数据',3,N'editentitylink',N'')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Element]([Id],[Name],[ElementType],[Identification],[Route])VALUES(N'90617210-63ee-4d54-3656-08d709bf0b65',N'删除数据',3,N'deletedatalink',N'')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'260632bd-6db9-4d83-4e7b-08d709bf1c3c',N'96469b51-7236-48c4-363c-08d709bf0b65',N'afb4c264-b57a-4936-7d79-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'3ca2b96a-3d90-4730-4e7c-08d709bf1c3c',N'c1be82cf-cf72-425f-363d-08d709bf0b65',N'3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'9fab1859-d95c-43d8-4e7d-08d709bf1c3c',N'88877f44-13f8-41c5-363e-08d709bf0b65',N'65f688c1-c679-4d38-7d7c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'c5695b43-fa18-465b-4e7e-08d709bf1c3c',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',N'40998129-255c-4edf-7d7a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'2dcba122-99e1-4474-4e7f-08d709bf1c3c',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',N'3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'928c7718-d4af-437b-4e80-08d709bf1c3c',N'3f70983f-1884-49ad-3640-08d709bf0b65',N'7ec86556-becf-49f3-7d7b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'7d8eb0fb-70d0-4e45-4e81-08d709bf1c3c',N'c27b893b-796d-4bbf-3641-08d709bf0b65',N'6330e0bd-42fc-4ac6-7d71-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'b25b7a31-b47f-48e0-4e82-08d709bf1c3c',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',N'26b0f3a0-dd75-4e44-7d6f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'946d4682-3fcc-46e2-4e83-08d709bf1c3c',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',N'90678f43-39c7-4ccf-7d72-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'cadb4256-8dcc-4427-4e84-08d709bf1c3c',N'f41535d8-03ec-4005-3644-08d709bf0b65',N'0df18f48-730c-4398-7d70-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'950793cf-c461-4da3-4e85-08d709bf1c3c',N'5d542eef-5b8a-47b4-3645-08d709bf0b65',N'22e89a74-3e03-4f1d-7d6b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'8b494bd4-dee1-438b-4e86-08d709bf1c3c',N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',N'a97d11cb-a691-4567-7d6c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'e6edba8e-a8cd-4b5a-4e87-08d709bf1c3c',N'6f9ae434-29bc-43b6-3647-08d709bf0b65',N'0273a51b-6a4e-4b77-7d6a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'64c6e573-b134-41e6-4e88-08d709bf1c3c',N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'598f8a41-6857-4012-7d69-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'3718cd91-9120-4e01-4e89-08d709bf1c3c',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'75e133dc-63ae-4027-7d6e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'4fe04787-cdc3-43d7-4e8a-08d709bf1c3c',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'ecc7625d-2763-42aa-4e8b-08d709bf1c3c',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'5241e0d6-0c82-4377-4e8c-08d709bf1c3c',N'f3e3bc58-3808-429c-3648-08d709bf0b65',N'3e76b4cf-0ff3-4b75-7d61-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'2471b4f9-dfe7-456b-4e8d-08d709bf1c3c',N'0161d8ed-91de-48d2-3649-08d709bf0b65',N'7d619e77-c7c4-446b-7d63-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'90e599e0-f8e3-4f8d-4e8f-08d709bf1c3c',N'fbcb28e3-36b3-4227-364a-08d709bf0b65',N'ff573676-00ce-46cf-7d62-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'f1889d5b-8ec3-402a-4e90-08d709bf1c3c',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'5b755102-503b-43bc-4e91-08d709bf1c3c',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'b23935af-93a3-4eee-7d5b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'531d0c51-5000-4046-4e92-08d709bf1c3c',N'f9a8bd61-348e-4730-364b-08d709bf0b65',N'308b4d8f-5db1-47e4-7d5d-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'c527df14-8450-4156-4e93-08d709bf1c3c',N'3d51d0dc-592d-4263-364c-08d709bf0b65',N'edfd819d-2424-446f-7d5f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'ba3cb147-fb3e-42fa-4e94-08d709bf1c3c',N'6ce2bc0b-951e-4818-364d-08d709bf0b65',N'cb72e62b-60cb-4188-7d5e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'b9a75303-660d-468b-4e95-08d709bf1c3c',N'b08a4c30-d492-4537-fa72-08d7072f517b',N'ec48a0c4-ffed-43ad-7d5c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'c60df8ff-eec9-4a8d-4e97-08d709bf1c3c',N'29c3b92a-2252-42fc-3652-08d709bf0b65',N'09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'd17266d9-c47f-4893-4e98-08d709bf1c3c',N'f72a8122-cde0-4e65-3653-08d709bf0b65',N'34a24768-5d7b-4ead-7d75-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'e9f2ddf3-76fc-44e6-4e9a-08d709bf1c3c',N'f736892d-c4a7-4012-3651-08d709bf0b65',N'09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'41fb7b3e-99a8-4893-4e9b-08d709bf1c3c',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'12456241-2644-4b13-4e9c-08d709bf1c3c',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'7a90f207-ec2a-435b-7d64-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'd5450a3b-95e9-4673-4e9d-08d709bf1c3c',N'e6e3efe1-10f0-4078-3654-08d709bf0b65',N'3f047f21-0c05-4f78-7d66-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'94058c04-0681-4f46-4e9e-08d709bf1c3c',N'a93f70c4-61b9-481f-3655-08d709bf0b65',N'1cbfc955-a4a4-48a2-7d67-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'0e6b4ff4-70b3-48d2-4e9f-08d709bf1c3c',N'90617210-63ee-4d54-3656-08d709bf0b65',N'5b3acd28-4d57-46ce-7d68-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'3b13385b-1c4d-4de9-4ea0-08d709bf1c3c',N'515f8064-e91c-41e2-fa74-08d7072f517b',N'b4d9c97c-a17d-48e5-7d65-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'd42a94ac-01b5-4848-361c-08d709ca74e9',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'5b343f0f-d587-4e10-7d77-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementApi]([Id],[ElementId],[ApiId])VALUES(N'a98623fb-55ef-4aed-361d-08d709ca74e9',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'b809dfc3-7af0-43c1-7d6d-08d708fe7bef')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'05e877a1-d897-4a68-6d14-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'b66e1bef-da5d-4ad1-6d15-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'2b83f45d-0b18-44fb-6d16-08d7072f517f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'cdbaf940-e8ae-4fe9-6d17-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'80f5ef02-0277-4d95-6d18-08d7072f517f',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'd059c25b-ba9f-4ac0-6d1c-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'62663b74-a5d0-43ed-fa70-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'29fdbb81-ed7d-4f22-6d1d-08d7072f517f',N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'62663b74-a5d0-43ed-fa70-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'39d441b8-9a2b-47dd-6d1e-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'5b859edf-eac7-46a7-fa71-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'c8ae766b-e077-4e7b-6d1f-08d7072f517f',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'5b859edf-eac7-46a7-fa71-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'f6a9a41f-59d9-4b07-6d20-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'b08a4c30-d492-4537-fa72-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'4274a4dd-9660-4906-6d21-08d7072f517f',N'b08a4c30-d492-4537-fa72-08d7072f517b',N'b08a4c30-d492-4537-fa72-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'f37856a6-6221-4630-6d22-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'41c5a517-75a4-4834-6d23-08d7072f517f',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'69253c30-9f02-4fc9-6d24-08d7072f517f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'515f8064-e91c-41e2-fa74-08d7072f517b',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'487f5dd0-65a0-4365-6d25-08d7072f517f',N'515f8064-e91c-41e2-fa74-08d7072f517b',N'515f8064-e91c-41e2-fa74-08d7072f517b',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'd425521c-60ba-4d2d-a94d-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'96469b51-7236-48c4-363c-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'87bdeded-2ee7-4597-a94e-08d709bf0b6f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'96469b51-7236-48c4-363c-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'c3bbfcd2-f730-4602-a94f-08d709bf0b6f',N'96469b51-7236-48c4-363c-08d709bf0b65',N'96469b51-7236-48c4-363c-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'e29b8667-292b-42ce-a950-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'c1be82cf-cf72-425f-363d-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'0a866110-9f55-4b51-a951-08d709bf0b6f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'c1be82cf-cf72-425f-363d-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'455b67bc-e2be-4b63-a952-08d709bf0b6f',N'c1be82cf-cf72-425f-363d-08d709bf0b65',N'c1be82cf-cf72-425f-363d-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'c2972e6c-63ee-4fcf-a953-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'88877f44-13f8-41c5-363e-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'5482fb60-7240-48d0-a954-08d709bf0b6f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'88877f44-13f8-41c5-363e-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'23bb2e14-5b04-46e1-a955-08d709bf0b6f',N'88877f44-13f8-41c5-363e-08d709bf0b65',N'88877f44-13f8-41c5-363e-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'83833d08-fa3c-46d2-a956-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'974d3101-6579-45ee-a957-08d709bf0b6f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'd729c043-2626-43ac-a958-08d709bf0b6f',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',N'b3440e7f-5aa1-424a-363f-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'9b83a5f5-4e0f-45aa-a959-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'3f70983f-1884-49ad-3640-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'65a8a5bc-7af3-47dc-a95a-08d709bf0b6f',N'ad592b36-dd5b-447b-fa6d-08d7072f517b',N'3f70983f-1884-49ad-3640-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'8cf18883-822b-4932-a95b-08d709bf0b6f',N'3f70983f-1884-49ad-3640-08d709bf0b65',N'3f70983f-1884-49ad-3640-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'3b6c8e81-ec3a-4f4b-a95c-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'c27b893b-796d-4bbf-3641-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'6605567b-c8b7-4aee-a95d-08d709bf0b6f',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'c27b893b-796d-4bbf-3641-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'531141eb-c5dd-424b-a95e-08d709bf0b6f',N'c27b893b-796d-4bbf-3641-08d709bf0b65',N'c27b893b-796d-4bbf-3641-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'09b1cf87-cf84-4b38-a962-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'7bb16f1e-216b-4932-a963-08d709bf0b6f',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'6c2e601f-0ebc-4656-a964-08d709bf0b6f',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',N'7c3e6882-22c3-45e7-3643-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'941b16c5-6f9c-409a-a965-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'f41535d8-03ec-4005-3644-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'c07d5b17-2001-4e2f-a966-08d709bf0b6f',N'7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'f41535d8-03ec-4005-3644-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'80f6eabd-5b15-4320-a967-08d709bf0b6f',N'f41535d8-03ec-4005-3644-08d709bf0b65',N'f41535d8-03ec-4005-3644-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'82f153c7-36db-497c-a968-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'5d542eef-5b8a-47b4-3645-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'120852ef-7ea3-4008-a969-08d709bf0b6f',N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'5d542eef-5b8a-47b4-3645-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'89402946-0c4e-402d-a96a-08d709bf0b6f',N'5d542eef-5b8a-47b4-3645-08d709bf0b65',N'5d542eef-5b8a-47b4-3645-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'346e9d28-09ab-4bb3-a96b-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'7afdc9ec-ab50-4f7a-a96c-08d709bf0b6f',N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'fd90f5ae-0207-4da2-a96d-08d709bf0b6f',N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',N'a0e99d34-8dc2-4fe1-3646-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'3fc5db84-aa00-4939-a96e-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'6f9ae434-29bc-43b6-3647-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'1d44302b-5f9b-42ff-a96f-08d709bf0b6f',N'62663b74-a5d0-43ed-fa70-08d7072f517b',N'6f9ae434-29bc-43b6-3647-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'a7427846-7e3a-4e9d-a970-08d709bf0b6f',N'6f9ae434-29bc-43b6-3647-08d709bf0b65',N'6f9ae434-29bc-43b6-3647-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'88b1b3ba-4b2a-42e5-a971-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'f3e3bc58-3808-429c-3648-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'b8092fb7-3a59-44a8-a972-08d709bf0b6f',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'f3e3bc58-3808-429c-3648-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'dbb4d014-5fe9-4475-a973-08d709bf0b6f',N'f3e3bc58-3808-429c-3648-08d709bf0b65',N'f3e3bc58-3808-429c-3648-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'1ca9e923-04a9-4f85-a974-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'0161d8ed-91de-48d2-3649-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'2ab6fb14-b3a3-4035-a975-08d709bf0b6f',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'0161d8ed-91de-48d2-3649-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'392a406f-b17c-4fe9-a976-08d709bf0b6f',N'0161d8ed-91de-48d2-3649-08d709bf0b65',N'0161d8ed-91de-48d2-3649-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'9b84c68c-af3d-41f5-a977-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'fbcb28e3-36b3-4227-364a-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'deee1c1a-4747-484b-a978-08d709bf0b6f',N'5b859edf-eac7-46a7-fa71-08d7072f517b',N'fbcb28e3-36b3-4227-364a-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'c0824065-945b-41f7-a979-08d709bf0b6f',N'fbcb28e3-36b3-4227-364a-08d709bf0b65',N'fbcb28e3-36b3-4227-364a-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'225723a8-705c-45c6-a97a-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'f9a8bd61-348e-4730-364b-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'6d307c00-b49d-456d-a97b-08d709bf0b6f',N'b08a4c30-d492-4537-fa72-08d7072f517b',N'f9a8bd61-348e-4730-364b-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'8a694ade-afab-4d8c-a97c-08d709bf0b6f',N'f9a8bd61-348e-4730-364b-08d709bf0b65',N'f9a8bd61-348e-4730-364b-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'e655b031-c778-41b7-a97d-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'3d51d0dc-592d-4263-364c-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'0e4f1199-5bcd-4e08-a97e-08d709bf0b6f',N'b08a4c30-d492-4537-fa72-08d7072f517b',N'3d51d0dc-592d-4263-364c-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'56594a89-2f5c-417a-a97f-08d709bf0b6f',N'3d51d0dc-592d-4263-364c-08d709bf0b65',N'3d51d0dc-592d-4263-364c-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'688ff1e0-6409-42ec-a980-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'6ce2bc0b-951e-4818-364d-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'9a8204d8-0733-42ac-a981-08d709bf0b6f',N'b08a4c30-d492-4537-fa72-08d7072f517b',N'6ce2bc0b-951e-4818-364d-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'd51841ed-f853-4d1f-a982-08d709bf0b6f',N'6ce2bc0b-951e-4818-364d-08d709bf0b65',N'6ce2bc0b-951e-4818-364d-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'dee0d080-202c-4d1e-a983-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'f736892d-c4a7-4012-3651-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'57c31bee-6973-4d89-a984-08d709bf0b6f',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'f736892d-c4a7-4012-3651-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'1ecff558-6a3a-4b72-a985-08d709bf0b6f',N'f736892d-c4a7-4012-3651-08d709bf0b65',N'f736892d-c4a7-4012-3651-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'ad5fae0d-1388-46b4-a986-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'29c3b92a-2252-42fc-3652-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'8739e373-4d70-433d-a987-08d709bf0b6f',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'29c3b92a-2252-42fc-3652-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'3d5c2071-3360-4a48-a988-08d709bf0b6f',N'29c3b92a-2252-42fc-3652-08d709bf0b65',N'29c3b92a-2252-42fc-3652-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'd2a63141-b34e-4609-a989-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'f72a8122-cde0-4e65-3653-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'b2200994-cbad-4c0b-a98a-08d709bf0b6f',N'e68887e2-1f67-4ee3-fa73-08d7072f517b',N'f72a8122-cde0-4e65-3653-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'6b2f9ed9-833c-4bf5-a98b-08d709bf0b6f',N'f72a8122-cde0-4e65-3653-08d709bf0b65',N'f72a8122-cde0-4e65-3653-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'f1ebe3fe-7cdd-4d79-a98c-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'e6e3efe1-10f0-4078-3654-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'7dcbabbe-b41c-4b02-a98d-08d709bf0b6f',N'515f8064-e91c-41e2-fa74-08d7072f517b',N'e6e3efe1-10f0-4078-3654-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'5ab5227a-a6cb-4a3f-a98e-08d709bf0b6f',N'e6e3efe1-10f0-4078-3654-08d709bf0b65',N'e6e3efe1-10f0-4078-3654-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'61f5001a-d6cc-4055-a98f-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'a93f70c4-61b9-481f-3655-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'05f08ea7-e6ab-446f-a990-08d709bf0b6f',N'515f8064-e91c-41e2-fa74-08d7072f517b',N'a93f70c4-61b9-481f-3655-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'dd7eac0b-7f14-4769-a991-08d709bf0b6f',N'a93f70c4-61b9-481f-3655-08d709bf0b65',N'a93f70c4-61b9-481f-3655-08d709bf0b65',0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'e54fdc52-d0a9-475d-a992-08d709bf0b6f',N'c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'90617210-63ee-4d54-3656-08d709bf0b65',2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'ae2a0b11-1fa2-4932-a993-08d709bf0b6f',N'515f8064-e91c-41e2-fa74-08d7072f517b',N'90617210-63ee-4d54-3656-08d709bf0b65',1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO[dbo].[ElementTree]([Id],[Ancestor],[Descendant],[Length])VALUES(N'f888d3cf-de89-4ed1-a994-08d709bf0b6f',N'90617210-63ee-4d54-3656-08d709bf0b65',N'90617210-63ee-4d54-3656-08d709bf0b65',0)");


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
