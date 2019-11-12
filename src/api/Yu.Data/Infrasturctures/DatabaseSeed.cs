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
using Yu.Data.Infrasturctures.BaseIdentity;

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

                    // 初始化其他数据
                    InitOtherData(dbContext);

                    //UpdateApi(dbContext, app);

                    // 初始化实体数据表
                    InitEntityData(dbContext);

                    // 初始化成员和角色
                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<BaseIdentityUser>>();
                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<BaseIdentityRole>>();
                    InitMember(userManager, roleManager);

                    // 自定义初始方法
                    dataSeed?.Invoke(dbContext);
                }
                else
                {
                    //var dbContext = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();

                    // 更新API数据
                    //UpdateApi(dbContext, app);

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
            var xing = new List<string>() {"赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈", "楮", "卫", "蒋", "沈", "韩", "杨",
        "朱", "秦", "尤", "许", "何", "吕", "施", "张", "孔", "曹", "严", "华", "金", "魏", "陶", "姜",
        "戚", "谢", "邹", "喻", "柏", "水", "窦", "章", "云", "苏", "潘", "葛", "奚", "范", "彭", "郎",
        "鲁", "韦", "昌", "马", "苗", "凤", "花", "方", "俞", "任", "袁", "柳", "酆", "鲍", "史", "唐",
        "费", "廉", "岑", "薛", "雷", "贺", "倪", "汤", "滕", "殷", "罗", "毕", "郝", "邬", "安", "常",
        "乐", "于", "时", "傅", "皮", "卞", "齐", "康", "伍", "余", "元", "卜", "顾", "孟", "平", "黄",
        "和", "穆", "萧", "尹", "姚", "邵", "湛", "汪", "祁", "毛", "禹", "狄", "米", "贝", "明", "臧",
        "计", "伏", "成", "戴", "谈", "宋", "茅", "庞", "熊", "纪", "舒", "屈", "项", "祝", "董", "梁",
        "杜", "阮", "蓝", "闽", "席", "季", "麻", "强", "贾", "路", "娄", "危", "江", "童", "颜", "郭",
        "梅", "盛", "林", "刁", "锺", "徐", "丘", "骆", "高", "夏", "蔡", "田", "樊", "胡", "凌", "霍",
        "虞", "万", "支", "柯", "昝", "管", "卢", "莫", "经", "房", "裘", "缪", "干", "解", "应", "宗",
        "丁", "宣", "贲", "邓", "郁", "单", "杭", "洪", "包", "诸", "左", "石", "崔", "吉", "钮", "龚",
        "程", "嵇", "邢", "滑", "裴", "陆", "荣", "翁", "荀", "羊", "於", "惠", "甄", "麹", "家", "封",
        "芮", "羿", "储", "靳", "汲", "邴", "糜", "松", "井", "段", "富", "巫", "乌", "焦", "巴", "弓",
        "牧", "隗", "山", "谷", "车", "侯", "宓", "蓬", "全", "郗", "班", "仰", "秋", "仲", "伊", "宫",
        "宁", "仇", "栾", "暴", "甘", "斜", "厉", "戎", "祖", "武", "符", "刘", "景", "詹", "束", "龙" };
            var random = new Random(DateTime.Now.Millisecond);
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
                    Roles = "系统管理员",
                    FullName = xing[random.Next(0, xing.Count)] + xing[random.Next(0, xing.Count)],
                    CreateTime = DateTime.Now
                };
                userManager.CreateAsync(user, CommonConstants.Password).Wait();
                userManager.AddToRoleAsync(user, "系统管理员").Wait();
            }
        }

        // 初始化页面权限数据
        private static void InitOtherData<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('c5fdd1a3-aafe-430e-fa6c-08d7072f517b', N'权限管理', 1, 'rightmanage', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('ad592b36-dd5b-447b-fa6d-08d7072f517b', N'用户管理', 1, 'usermanage', '/right/user')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('7bfaa83d-8611-4fcd-fa6e-08d7072f517b', N'角色管理', 1, 'rolemanage', '/right/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('62663b74-a5d0-43ed-fa70-08d7072f517b', N'组织管理', 1, 'groupmanage', '/right/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('b0c12bbd-cb1d-4125-fa70-08d7072f517b', N'岗位管理', 1, 'positionmanage', '/right/menu')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('5b859edf-eac7-46a7-fa71-08d7072f517b', N'页面元素管理', 1, 'elementmanage', '/right/menu')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('b08a4c30-d492-4537-fa72-08d7072f517b', N'api数据管理', 1, 'apimanage', '/right/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('e68887e2-1f67-4ee3-fa73-08d7072f517b', N'规则管理', 1, 'rulemanage', '/right/rule')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('515f8064-e91c-41e2-fa74-08d7072f517b', N'实体数据管理', 1, 'entitymanage', '/right/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('96469b51-7236-48c4-363c-08d709bf0b65', N'添加用户', 2, 'adduserbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('c1be82cf-cf72-425f-363d-08d709bf0b65', N'查看用户', 3, 'viewuserlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('88877f44-13f8-41c5-363e-08d709bf0b65', N'编辑头像', 3, 'editavatarlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('b3440e7f-5aa1-424a-363f-08d709bf0b65', N'编辑信息', 3, 'edituserlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('3f70983f-1884-49ad-3640-08d709bf0b65', N'删除用户', 3, 'deleteuserlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('c27b893b-796d-4bbf-3641-08d709bf0b65', N'添加角色', 2, 'addrolebtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('7c3e6882-22c3-45e7-3643-08d709bf0b65', N'修改角色', 3, 'editrolelink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('f41535d8-03ec-4005-3644-08d709bf0b65', N'删除角色', 3, 'deleterolelink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('5d542eef-5b8a-47b4-3645-08d709bf0b65', N'创建组织', 2, 'addgroupbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('a0e99d34-8dc2-4fe1-3646-08d709bf0b65', N'修改组织', 2, 'editgroupbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('6f9ae434-29bc-43b6-3647-08d709bf0b65', N'删除组织', 2, 'deletegroupbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('f3e3bc58-3808-429c-3648-08d709bf0b65', N'添加元素', 2, 'addelementbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('0161d8ed-91de-48d2-3649-08d709bf0b65', N'修改元素', 2, 'editelementbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('fbcb28e3-36b3-4227-364a-08d709bf0b65', N'删除元素', 2, 'deleteelementbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('f9a8bd61-348e-4730-364b-08d709bf0b65', N'添加api', 2, 'addapibtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('3d51d0dc-592d-4263-364c-08d709bf0b65', N'删除api', 3, 'deleteapilink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('6ce2bc0b-951e-4818-364d-08d709bf0b65', N'修改api', 3, 'editapilink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('f736892d-c4a7-4012-3651-08d709bf0b65', N'添加规则', 2, 'addrulegroupbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('29c3b92a-2252-42fc-3652-08d709bf0b65', N'编辑规则', 3, 'editrulegroup', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('f72a8122-cde0-4e65-3653-08d709bf0b65', N'删除规则', 3, 'deleterulegrouplink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('e6e3efe1-10f0-4078-3654-08d709bf0b65', N'添加数据', 2, 'addentitybtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('a93f70c4-61b9-481f-3655-08d709bf0b65', N'修改数据', 3, 'editentitylink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('90617210-63ee-4d54-3656-08d709bf0b65', N'删除数据', 3, 'deletedatalink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('b4829b5c-715c-44c3-ee2c-08d7618e356c', N'添加岗位', 2, 'addpositionbtn', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('1ec0e308-6076-4759-ee2d-08d7618e356c', N'删除', 3, 'deletepositionlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('ece2bc32-4a1b-4287-ee2e-08d7618e356c', N'修改', 3, 'editpositionlink', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('36300b49-904f-425b-ee2f-08d7618e356c', N'工作流', 1, 'workflow', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('7d812a44-3387-4f30-34c0-08d76192cd65', N'使用', 1, 'wfuse', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('da76f33b-3a63-4831-34c1-08d76192cd65', N'管理', 1, 'wfm', '')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('97c4e48b-159a-4a54-34c2-08d76192cd65', N'我的工作', 1, 'mywf', '/workflow/job')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('1176960a-ea52-4dc2-34c3-08d76192cd65', N'我的待办', 1, 'myhandle', '/workflow/handle')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('9a860fc5-05e1-471b-34c4-08d76192cd65', N'回收站', 1, 'wfrecyclebin', '/workflow/recycle')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Element\" (\"Id\", \"Name\", \"ElementType\", \"Identification\", \"Route\") VALUES ('7d83ef7c-c41d-4819-34c5-08d76192cd65', N'工作流管理', 1, 'wfmanage', '/workflow/definition')");


            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('05e877a1-d897-4a68-6d14-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b66e1bef-da5d-4ad1-6d15-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('2b83f45d-0b18-44fb-6d16-08d7072f517f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('cdbaf940-e8ae-4fe9-6d17-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('80f5ef02-0277-4d95-6d18-08d7072f517f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d059c25b-ba9f-4ac0-6d1c-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '62663b74-a5d0-43ed-fa70-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('29fdbb81-ed7d-4f22-6d1d-08d7072f517f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '62663b74-a5d0-43ed-fa70-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('39d441b8-9a2b-47dd-6d1e-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '5b859edf-eac7-46a7-fa71-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c8ae766b-e077-4e7b-6d1f-08d7072f517f', '5b859edf-eac7-46a7-fa71-08d7072f517b', '5b859edf-eac7-46a7-fa71-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f6a9a41f-59d9-4b07-6d20-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b08a4c30-d492-4537-fa72-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('4274a4dd-9660-4906-6d21-08d7072f517f', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'b08a4c30-d492-4537-fa72-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f37856a6-6221-4630-6d22-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('41c5a517-75a4-4834-6d23-08d7072f517f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('69253c30-9f02-4fc9-6d24-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '515f8064-e91c-41e2-fa74-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('487f5dd0-65a0-4365-6d25-08d7072f517f', '515f8064-e91c-41e2-fa74-08d7072f517b', '515f8064-e91c-41e2-fa74-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d425521c-60ba-4d2d-a94d-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '96469b51-7236-48c4-363c-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('87bdeded-2ee7-4597-a94e-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '96469b51-7236-48c4-363c-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c3bbfcd2-f730-4602-a94f-08d709bf0b6f', '96469b51-7236-48c4-363c-08d709bf0b65', '96469b51-7236-48c4-363c-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e29b8667-292b-42ce-a950-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('0a866110-9f55-4b51-a951-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('455b67bc-e2be-4b63-a952-08d709bf0b6f', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c2972e6c-63ee-4fcf-a953-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '88877f44-13f8-41c5-363e-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('5482fb60-7240-48d0-a954-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '88877f44-13f8-41c5-363e-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('23bb2e14-5b04-46e1-a955-08d709bf0b6f', '88877f44-13f8-41c5-363e-08d709bf0b65', '88877f44-13f8-41c5-363e-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('83833d08-fa3c-46d2-a956-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('974d3101-6579-45ee-a957-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d729c043-2626-43ac-a958-08d709bf0b6f', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9b83a5f5-4e0f-45aa-a959-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '3f70983f-1884-49ad-3640-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('65a8a5bc-7af3-47dc-a95a-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '3f70983f-1884-49ad-3640-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8cf18883-822b-4932-a95b-08d709bf0b6f', '3f70983f-1884-49ad-3640-08d709bf0b65', '3f70983f-1884-49ad-3640-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3b6c8e81-ec3a-4f4b-a95c-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6605567b-c8b7-4aee-a95d-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('531141eb-c5dd-424b-a95e-08d709bf0b6f', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('09b1cf87-cf84-4b38-a962-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7bb16f1e-216b-4932-a963-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6c2e601f-0ebc-4656-a964-08d709bf0b6f', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('941b16c5-6f9c-409a-a965-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f41535d8-03ec-4005-3644-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c07d5b17-2001-4e2f-a966-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'f41535d8-03ec-4005-3644-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('80f6eabd-5b15-4320-a967-08d709bf0b6f', 'f41535d8-03ec-4005-3644-08d709bf0b65', 'f41535d8-03ec-4005-3644-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('82f153c7-36db-497c-a968-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('120852ef-7ea3-4008-a969-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('89402946-0c4e-402d-a96a-08d709bf0b6f', '5d542eef-5b8a-47b4-3645-08d709bf0b65', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('346e9d28-09ab-4bb3-a96b-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7afdc9ec-ab50-4f7a-a96c-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('fd90f5ae-0207-4da2-a96d-08d709bf0b6f', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3fc5db84-aa00-4939-a96e-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1d44302b-5f9b-42ff-a96f-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('a7427846-7e3a-4e9d-a970-08d709bf0b6f', '6f9ae434-29bc-43b6-3647-08d709bf0b65', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('88b1b3ba-4b2a-42e5-a971-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b8092fb7-3a59-44a8-a972-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dbb4d014-5fe9-4475-a973-08d709bf0b6f', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1ca9e923-04a9-4f85-a974-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '0161d8ed-91de-48d2-3649-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('2ab6fb14-b3a3-4035-a975-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', '0161d8ed-91de-48d2-3649-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('392a406f-b17c-4fe9-a976-08d709bf0b6f', '0161d8ed-91de-48d2-3649-08d709bf0b65', '0161d8ed-91de-48d2-3649-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9b84c68c-af3d-41f5-a977-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('deee1c1a-4747-484b-a978-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c0824065-945b-41f7-a979-08d709bf0b6f', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('225723a8-705c-45c6-a97a-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6d307c00-b49d-456d-a97b-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8a694ade-afab-4d8c-a97c-08d709bf0b6f', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e655b031-c778-41b7-a97d-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '3d51d0dc-592d-4263-364c-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('0e4f1199-5bcd-4e08-a97e-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', '3d51d0dc-592d-4263-364c-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('56594a89-2f5c-417a-a97f-08d709bf0b6f', '3d51d0dc-592d-4263-364c-08d709bf0b65', '3d51d0dc-592d-4263-364c-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('688ff1e0-6409-42ec-a980-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9a8204d8-0733-42ac-a981-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d51841ed-f853-4d1f-a982-08d709bf0b6f', '6ce2bc0b-951e-4818-364d-08d709bf0b65', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dee0d080-202c-4d1e-a983-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f736892d-c4a7-4012-3651-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('57c31bee-6973-4d89-a984-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'f736892d-c4a7-4012-3651-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1ecff558-6a3a-4b72-a985-08d709bf0b6f', 'f736892d-c4a7-4012-3651-08d709bf0b65', 'f736892d-c4a7-4012-3651-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ad5fae0d-1388-46b4-a986-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '29c3b92a-2252-42fc-3652-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8739e373-4d70-433d-a987-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '29c3b92a-2252-42fc-3652-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3d5c2071-3360-4a48-a988-08d709bf0b6f', '29c3b92a-2252-42fc-3652-08d709bf0b65', '29c3b92a-2252-42fc-3652-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d2a63141-b34e-4609-a989-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b2200994-cbad-4c0b-a98a-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6b2f9ed9-833c-4bf5-a98b-08d709bf0b6f', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f1ebe3fe-7cdd-4d79-a98c-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7dcbabbe-b41c-4b02-a98d-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('5ab5227a-a6cb-4a3f-a98e-08d709bf0b6f', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('61f5001a-d6cc-4055-a98f-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('05f08ea7-e6ab-446f-a990-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dd7eac0b-7f14-4769-a991-08d709bf0b6f', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e54fdc52-d0a9-475d-a992-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '90617210-63ee-4d54-3656-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ae2a0b11-1fa2-4932-a993-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', '90617210-63ee-4d54-3656-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f888d3cf-de89-4ed1-a994-08d709bf0b6f', '90617210-63ee-4d54-3656-08d709bf0b65', '90617210-63ee-4d54-3656-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ebcad59c-2378-412e-1a5d-08d7618e357f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('a0c75239-7c33-43ed-1a5e-08d7618e357f', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b58281b2-96bd-4a9a-1a5f-08d7618e357f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('01420b1e-0249-4785-1a60-08d7618e357f', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('012b6c70-e47a-4efe-1a61-08d7618e357f', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ebacc28b-7826-48e5-1a62-08d7618e357f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '1ec0e308-6076-4759-ee2d-08d7618e356c', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('760fe3b7-da1e-4dd5-1a63-08d7618e357f', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', '1ec0e308-6076-4759-ee2d-08d7618e356c', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('21c6d99b-0d27-42ef-1a64-08d7618e357f', '1ec0e308-6076-4759-ee2d-08d7618e356c', '1ec0e308-6076-4759-ee2d-08d7618e356c', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8c7d412b-5a93-44bb-1a65-08d7618e357f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'ece2bc32-4a1b-4287-ee2e-08d7618e356c', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('de316253-6406-4ab2-1a66-08d7618e357f', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 'ece2bc32-4a1b-4287-ee2e-08d7618e356c', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('efa7c604-90c3-45f0-1a67-08d7618e357f', 'ece2bc32-4a1b-4287-ee2e-08d7618e356c', 'ece2bc32-4a1b-4287-ee2e-08d7618e356c', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('10736002-3c46-4605-1a68-08d7618e357f', '36300b49-904f-425b-ee2f-08d7618e356c', '36300b49-904f-425b-ee2f-08d7618e356c', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6a206998-6304-4724-aeaa-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', '7d812a44-3387-4f30-34c0-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c02e704d-1728-4eb0-aeab-08d76192cd73', '7d812a44-3387-4f30-34c0-08d76192cd65', '7d812a44-3387-4f30-34c0-08d76192cd65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('33a8a9c4-fa70-48e1-aeac-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', 'da76f33b-3a63-4831-34c1-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1577a376-4558-4906-aead-08d76192cd73', 'da76f33b-3a63-4831-34c1-08d76192cd65', 'da76f33b-3a63-4831-34c1-08d76192cd65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6e9cf502-71fe-4ecc-aeae-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', '97c4e48b-159a-4a54-34c2-08d76192cd65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ced219a1-a42f-4f72-aeaf-08d76192cd73', '7d812a44-3387-4f30-34c0-08d76192cd65', '97c4e48b-159a-4a54-34c2-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3fb1c1e5-2647-4422-aeb0-08d76192cd73', '97c4e48b-159a-4a54-34c2-08d76192cd65', '97c4e48b-159a-4a54-34c2-08d76192cd65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b65af075-5d78-4aaa-aeb1-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', '1176960a-ea52-4dc2-34c3-08d76192cd65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('408cf77d-c46a-4743-aeb2-08d76192cd73', '7d812a44-3387-4f30-34c0-08d76192cd65', '1176960a-ea52-4dc2-34c3-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('99fa9e61-a276-4496-aeb3-08d76192cd73', '1176960a-ea52-4dc2-34c3-08d76192cd65', '1176960a-ea52-4dc2-34c3-08d76192cd65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('317952c0-4558-44bd-aeb4-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', '9a860fc5-05e1-471b-34c4-08d76192cd65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6773515f-f8c8-4c0f-aeb5-08d76192cd73', '7d812a44-3387-4f30-34c0-08d76192cd65', '9a860fc5-05e1-471b-34c4-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3b98a4df-f270-4989-aeb6-08d76192cd73', '9a860fc5-05e1-471b-34c4-08d76192cd65', '9a860fc5-05e1-471b-34c4-08d76192cd65', 0)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('24d468f7-5fca-4e7d-aeb7-08d76192cd73', '36300b49-904f-425b-ee2f-08d7618e356c', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 2)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('5e5bf97c-b10e-4436-aeb8-08d76192cd73', 'da76f33b-3a63-4831-34c1-08d76192cd65', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 1)");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('da178851-a70b-490e-aeb9-08d76192cd73', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 0)");

            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('9807863c-b234-4452-7d57-08d708fe7bef', N'账户管理-用户名密码登陆', N'Account', N'POST', N'/api/Account/Login')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('47317471-a6d6-4c6f-7d58-08d708fe7bef', N'账户管理-刷新token', N'Account', N'POST', N'/api/Account/RefreshToken')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8b02d6f4-c6e7-44ea-7d59-08d708fe7bef', N'账户管理-修改密码', N'Account', N'POST', N'/api/Account/ChangePwd')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('4768aa73-11a1-4016-7d5a-08d708fe7bef', N'验证码-获取验证码', N'Captcha', N'GET', N'/api/Captcha')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('b23935af-93a3-4eee-7d5b-08d708fe7bef', N'接口管理-取得全部Api数据', N'Api', N'GET', N'/api/allApi')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('ec48a0c4-ffed-43ad-7d5c-08d708fe7bef', N'接口管理-取得API数据', N'Api', N'GET', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('308b4d8f-5db1-47e4-7d5d-08d708fe7bef', N'接口管理-添加API数据', N'Api', N'POST', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('cb72e62b-60cb-4188-7d5e-08d708fe7bef', N'接口管理-更新api数据', N'Api', N'PUT', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('edfd819d-2424-446f-7d5f-08d708fe7bef', N'接口管理-删除api数据', N'Api', N'DELETE', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('fa75a6b9-743e-4961-7d60-08d708fe7bef', N'页面元素管理-取得页面元素', N'Element', N'GET', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('3e76b4cf-0ff3-4b75-7d61-08d708fe7bef', N'页面元素管理-添加新元素', N'Element', N'POST', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('ff573676-00ce-46cf-7d62-08d708fe7bef', N'页面元素管理-删除元素', N'Element', N'DELETE', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('7d619e77-c7c4-446b-7d63-08d708fe7bef', N'页面元素管理-更新元素', N'Element', N'PUT', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('7a90f207-ec2a-435b-7d64-08d708fe7bef', N'实体管理-取得实体数据(下拉框用)', N'Entity', N'GET', N'/api/entities')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('b4d9c97c-a17d-48e5-7d65-08d708fe7bef', N'实体管理-取得实体数据', N'Entity', N'GET', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('3f047f21-0c05-4f78-7d66-08d708fe7bef', N'实体管理-添加实体数据', N'Entity', N'POST', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('1cbfc955-a4a4-48a2-7d67-08d708fe7bef', N'实体管理-更新实体数据', N'Entity', N'PUT', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5b3acd28-4d57-46ce-7d68-08d708fe7bef', N'实体管理-删除实体数据', N'Entity', N'DELETE', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('598f8a41-6857-4012-7d69-08d708fe7bef', N'组织结构管理-取得所有组织', N'Group', N'GET', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('0273a51b-6a4e-4b77-7d6a-08d708fe7bef', N'组织结构管理-删除组织', N'Group', N'DELETE', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('22e89a74-3e03-4f1d-7d6b-08d708fe7bef', N'组织结构管理-创建组织', N'Group', N'POST', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a97d11cb-a691-4567-7d6c-08d708fe7bef', N'组织结构管理-更新组织', N'Group', N'PUT', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('b809dfc3-7af0-43c1-7d6d-08d708fe7bef', N'角色管理-取得全部角色名称', N'Role', N'GET', N'/api/roleNames')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('75e133dc-63ae-4027-7d6e-08d708fe7bef', N'角色管理-取得角色概要数据', N'Role', N'GET', N'/api/roleOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('26b0f3a0-dd75-4e44-7d6f-08d708fe7bef', N'角色管理-取得角色详细数据', N'Role', N'GET', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('0df18f48-730c-4398-7d70-08d708fe7bef', N'角色管理-删除角色数据', N'Role', N'DELETE', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('6330e0bd-42fc-4ac6-7d71-08d708fe7bef', N'角色管理-创建角色', N'Role', N'POST', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('90678f43-39c7-4ccf-7d72-08d708fe7bef', N'角色管理-更新角色', N'Role', N'PUT', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8488942f-ed09-4d55-7d73-08d708fe7bef', N'规则管理-查看所有规则组', N'Rule', N'GET', N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a6b958f7-5d93-456c-7d74-08d708fe7bef', N'规则管理-查看规则组内容', N'Rule', N'GET', N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('34a24768-5d7b-4ead-7d75-08d708fe7bef', N'规则管理-删除规则组', N'Rule', N'DELETE', N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('09861d8a-2d36-4d6f-7d76-08d708fe7bef', N'规则管理-添加修改规则组', N'Rule', N'PUT', N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5b343f0f-d587-4e10-7d77-08d708fe7bef', N'用户管理-取得用户概要情报', N'User', N'GET', N'/api/userOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('3220f72a-a4ca-4326-7d78-08d708fe7bef', N'用户管理-取得用户数据', N'User', N'GET', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('afb4c264-b57a-4936-7d79-08d708fe7bef', N'用户管理-添加新用户', N'User', N'POST', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('40998129-255c-4edf-7d7a-08d708fe7bef', N'用户管理-更新用户数据', N'User', N'PUT', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('7ec86556-becf-49f3-7d7b-08d708fe7bef', N'用户管理-删除用户数据', N'User', N'DELETE', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('65f688c1-c679-4d38-7d7c-08d708fe7bef', N'用户管理-设置用户头像', N'User', N'POST', N'api/userAvatar')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a8b1d559-3150-4872-aed1-08d7619a4e89', N'账户管理-用户名密码登陆', N'Account', N'POST', N'/api/Account/Login/')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('df8ea349-7bad-4bf7-aed2-08d7619a4e89', N'账户管理-刷新token', N'Account', N'POST', N'/api/Account/RefreshToken/')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e314f1a2-e1db-4503-aed3-08d7619a4e89', N'账户管理-手机号重置密码', N'Account', N'GET', N'/api/Account/ResetPwdByPhone/')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e05739e6-fb9e-41f1-aed4-08d7619a4e89', N'账户管理-手机号重置密码', N'Account', N'POST', N'/api/Account/ResetPwdByPhone/')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f436d769-ffb6-42f4-aed5-08d7619a4e89', N'验证码-获取验证码', N'Captcha', N'GET', N'/api/Captcha/')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('599e9098-09f7-4004-aed6-08d7619a4e89', N'个人信息-设置用户头像', N'UserInfo', N'POST', N'/api/userInfo/avatar')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e8f7b596-3b39-40f6-aed7-08d7619a4e89', N'个人信息-取得用户信息', N'UserInfo', N'GET', N'/api/userInfo')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8f9bc6d8-df7c-4433-aed8-08d7619a4e89', N'个人信息-修改密码', N'UserInfo', N'POST', N'/api/userInfo/password')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a95015fc-1704-4124-aed9-08d7619a4e89', N'工作流表单管理-取得工作流表单', N'WorkFlowForm', N'GET', N'/api/workflowForm')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2ce6bb53-4141-44c9-aeda-08d7619a4e89', N'工作流表单管理-取得工作流表单元素', N'WorkFlowForm', N'GET', N'/api/workflowFormElement')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5de7dfda-0ba1-4fbd-aedb-08d7619a4e89', N'工作流表单管理-更新工作流表单', N'WorkFlowForm', N'PUT', N'/api/workflowForm')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('692c92d8-b178-47ab-aedc-08d7619a4e89', N'工作流实例-取得工作流实例数据', N'WorkFlowInstance', N'GET', N'/api/workflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f44df964-6329-4096-aedd-08d7619a4e89', N'工作流实例-添加工作流实例数据', N'WorkFlowInstance', N'POST', N'/api/workflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('29a3a368-b670-48ec-aede-08d7619a4e89', N'工作流实例-更新工作流实例数据', N'WorkFlowInstance', N'PUT', N'/api/workflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a8d770f8-0e04-4a40-aedf-08d7619a4e89', N'工作流实例-删除工作流实例数据', N'WorkFlowInstance', N'DELETE', N'/api/workflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('66545a16-d05a-4841-aee0-08d7619a4e89', N'工作流实例-取得回收站工作流实例数据', N'WorkFlowInstance', N'GET', N'/api/deltetedWorkflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a0b29977-1ed6-4db4-aee1-08d7619a4e89', N'工作流实例-工作流实例加入回收站', N'WorkFlowInstance', N'PUT', N'/api/deltetedWorkflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('59f2e8e0-375a-4cd2-aee2-08d7619a4e89', N'工作流实例-工作流实例从回收站取回', N'WorkFlowInstance', N'PATCH', N'/api/deltetedWorkflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('3cc758fa-4529-4b6f-aee3-08d7619a4e89', N'工作流实例-取得工作流实例表单数据', N'WorkFlowInstance', N'GET', N'/api/workflowInstanceForm')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5b78755a-fc36-47b2-aee4-08d7619a4e89', N'工作流实例-更新工作流实例表单数据', N'WorkFlowInstance', N'PUT', N'/api/workflowInstanceForm')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('b0958d84-eaf7-44f0-aee5-08d7619a4e89', N'工作流实例-更新工作流实例表单文件', N'WorkFlowInstance', N'POST', N'/api/workFlowInstanceFormFile')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2700712d-7fd7-40ba-aee6-08d7619a4e89', N'工作流实例-删除工作流实例表单文件', N'WorkFlowInstance', N'DELETE', N'/api/workFlowInstanceFormFile')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f0530bfd-eb45-476b-aee7-08d7619a4e89', N'工作流实例-取得工作流节点处理数据', N'WorkFlowInstance', N'GET', N'/api/workflowInstanceNode')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('d730747d-e187-43e1-aee8-08d7619a4e89', N'工作流实例-处理工作流节点', N'WorkFlowInstance', N'PATCH', N'/api/workflowInstanceNode')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('45df68d7-64a3-4c0e-aee9-08d7619a4e89', N'工作流实例-取得待办工作流实例数据', N'WorkFlowInstance', N'GET', N'/api/handleWorkflowInstance')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f6bcf535-b52a-4477-aeea-08d7619a4e89', N'工作流实例-取得工作流节点元素设定', N'WorkFlowInstance', N'GET', N'/api/workflowNodeElement')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a8e8700e-4373-4f13-aeeb-08d7619a4e89', N'工作流定义-取得工作流定义', N'WorkFlowDefine', N'GET', N'/api/workflowDefine')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('be37712f-b5d3-4f57-aeec-08d7619a4e89', N'工作流定义-取得工作流定义列表', N'WorkFlowDefine', N'GET', N'/api/workflowDefines')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('aebaa69d-8115-444b-aeed-08d7619a4e89', N'工作流定义-取得全部工作流定义', N'WorkFlowDefine', N'GET', N'/api/allWorkflowDefines')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2adb0cb9-fbb0-4749-aeee-08d7619a4e89', N'工作流定义-添加工作流定义', N'WorkFlowDefine', N'POST', N'/api/workflowDefine')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('aaeaa10a-63ad-4617-aeef-08d7619a4e89', N'工作流定义-更新工作流定义', N'WorkFlowDefine', N'PUT', N'/api/workflowDefine')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f86f43eb-1668-4e56-aef0-08d7619a4e89', N'工作流定义-工作流定义发布', N'WorkFlowDefine', N'PATCH', N'/api/workflowDefine')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('0461bb95-e517-4b95-aef1-08d7619a4e89', N'工作流定义-删除工作流定义数据', N'WorkFlowDefine', N'DELETE', N'/api/workflowDefine')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('447d56cd-2c3a-4088-aef2-08d7619a4e89', N'工作流流程节点管理-取得工作流流程节点和连接', N'WorkFlowFlow', N'GET', N'/api/workflowFlow')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f46a5b73-dd16-40fd-aef3-08d7619a4e89', N'工作流流程节点管理-更新工作流流程节点和连接', N'WorkFlowFlow', N'PUT', N'/api/workflowFlow')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('25885f7d-8961-488f-aef4-08d7619a4e89', N'工作流管理-取得工作流类型数据', N'WorkFlowType', N'GET', N'/api/workflowType')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('3f1a6b0b-3978-4b22-aef5-08d7619a4e89', N'工作流管理-添加工作流类型数据', N'WorkFlowType', N'POST', N'/api/workflowType')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8f60955b-921a-4382-aef6-08d7619a4e89', N'工作流管理-更新工作流类型数据', N'WorkFlowType', N'PUT', N'/api/workflowType')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('949a6e14-8a93-4e7d-aef7-08d7619a4e89', N'工作流管理-删除工作流类型数据', N'WorkFlowType', N'DELETE', N'/api/workflowType')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('13743716-1165-4471-aef8-08d7619a4e89', N'接口管理-取得全部Api数据', N'Api', N'GET', N'/api/allApi')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('52a97b1d-1321-4963-aef9-08d7619a4e89', N'接口管理-取得API数据', N'Api', N'GET', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('1006db3b-c4ef-46aa-aefa-08d7619a4e89', N'接口管理-添加API数据', N'Api', N'POST', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('6e0130cd-074d-4815-aefb-08d7619a4e89', N'接口管理-更新api数据', N'Api', N'PUT', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2b6a63a7-46e6-4784-aefc-08d7619a4e89', N'接口管理-删除api数据', N'Api', N'DELETE', N'/api/api')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('62449229-b183-4e99-aefd-08d7619a4e89', N'页面元素管理-取得页面元素', N'Element', N'GET', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('21efa23d-dbbe-412f-aefe-08d7619a4e89', N'页面元素管理-添加新元素', N'Element', N'POST', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5ed2c2b6-a7a4-458f-aeff-08d7619a4e89', N'页面元素管理-删除元素', N'Element', N'DELETE', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('ffe6271e-d98f-4bd9-af00-08d7619a4e89', N'页面元素管理-更新元素', N'Element', N'PUT', N'/api/element')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('95b22102-990f-496b-af01-08d7619a4e89', N'实体管理-取得实体数据(下拉框用)', N'Entity', N'GET', N'/api/entities')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2b8b91cc-e8f0-467a-af02-08d7619a4e89', N'实体管理-取得实体数据', N'Entity', N'GET', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('2c9fb0d2-a3af-4c7b-af03-08d7619a4e89', N'实体管理-添加实体数据', N'Entity', N'POST', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('c4244155-2be6-45c8-af04-08d7619a4e89', N'实体管理-更新实体数据', N'Entity', N'PUT', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('f0911c0f-079b-461c-af05-08d7619a4e89', N'实体管理-删除实体数据', N'Entity', N'DELETE', N'/api/entity')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('48c24f97-6753-47c4-af06-08d7619a4e89', N'组织结构管理-取得所有组织', N'Group', N'GET', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('0d37f9b4-226d-441e-af07-08d7619a4e89', N'组织结构管理-删除组织', N'Group', N'DELETE', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('650fd37c-1292-48fb-af08-08d7619a4e89', N'组织结构管理-创建组织', N'Group', N'POST', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e746f485-eaa7-4664-af09-08d7619a4e89', N'组织结构管理-更新组织', N'Group', N'PUT', N'/api/group')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('4b58005c-07a9-4085-af0a-08d7619a4e89', N'岗位管理-取得岗位数据', N'Position', N'GET', N'/api/position')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('d604eb0f-541b-4a65-af0b-08d7619a4e89', N'岗位管理-取得岗位数据', N'Position', N'GET', N'/api/positions')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e0d35bdb-8e60-4f20-af0c-08d7619a4e89', N'岗位管理-添加岗位数据', N'Position', N'POST', N'/api/position')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('84db695c-de44-4629-af0d-08d7619a4e89', N'岗位管理-更新岗位数据', N'Position', N'PUT', N'/api/position')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('101931d4-9689-4745-af0e-08d7619a4e89', N'岗位管理-删除岗位数据', N'Position', N'DELETE', N'/api/position')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8cef7b63-99a1-45b3-af0f-08d7619a4e89', N'角色管理-取得全部角色名称', N'Role', N'GET', N'/api/roleNames')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('7741dde5-29a1-426f-af10-08d7619a4e89', N'角色管理-取得角色概要数据', N'Role', N'GET', N'/api/roleOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('d047e79c-4767-4b74-af11-08d7619a4e89', N'角色管理-取得角色详细数据', N'Role', N'GET', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('7db16b2c-7061-43f3-af12-08d7619a4e89', N'角色管理-删除角色数据', N'Role', N'DELETE', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('6ee71d4a-b891-420f-af13-08d7619a4e89', N'角色管理-创建角色', N'Role', N'POST', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('a11ed33a-ee0f-4bf3-af14-08d7619a4e89', N'角色管理-更新角色', N'Role', N'PUT', N'/api/role')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('1bb2bb48-b015-4581-af15-08d7619a4e89', N'规则管理-查看所有规则组', N'Rule', N'GET', N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e4320ee2-0686-4ea0-af16-08d7619a4e89', N'规则管理-查看规则组内容', N'Rule', N'GET', N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('d5dd119e-187e-4524-af17-08d7619a4e89', N'规则管理-删除规则组', N'Rule', N'DELETE', N'/api/ruleGroup')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('09fc1dff-48b8-4de3-af18-08d7619a4e89', N'规则管理-添加修改规则组', N'Rule', N'PUT', N'/api/ruleDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('e3124789-361a-47dc-af19-08d7619a4e89', N'用户管理-取得用户概要情报', N'User', N'GET', N'/api/userOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('70717ba8-d99c-4993-af1a-08d7619a4e89', N'用户管理-取得用户概要情报', N'User', N'GET', N'/api/assignOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('5faf0ac9-8111-4589-af1b-08d7619a4e89', N'用户管理-取得用户概要情报', N'User', N'GET', N'/api/positionOutline')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('14227ac5-c4b9-42ef-af1c-08d7619a4e89', N'用户管理-取得用户数据', N'User', N'GET', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('4e3de08b-eaac-47ed-af1d-08d7619a4e89', N'用户管理-添加新用户', N'User', N'POST', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('1d120735-ae2d-436e-af1e-08d7619a4e89', N'用户管理-更新用户数据', N'User', N'PUT', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('8ccfe69f-de13-456f-af1f-08d7619a4e89', N'用户管理-删除用户数据', N'User', N'DELETE', N'/api/userDetail')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"Api\" (\"Id\", \"Name\", \"Controller\", \"Type\", \"Address\") VALUES ('d4500f0f-f01e-4f45-af20-08d7619a4e89', N'用户管理-设置用户头像', N'User', N'POST', N'/api/userAvatar')");

            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('260632bd-6db9-4d83-4e7b-08d709bf1c3c', '96469b51-7236-48c4-363c-08d709bf0b65', 'afb4c264-b57a-4936-7d79-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3ca2b96a-3d90-4730-4e7c-08d709bf1c3c', 'c1be82cf-cf72-425f-363d-08d709bf0b65', '3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('9fab1859-d95c-43d8-4e7d-08d709bf1c3c', '88877f44-13f8-41c5-363e-08d709bf0b65', '65f688c1-c679-4d38-7d7c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c5695b43-fa18-465b-4e7e-08d709bf1c3c', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', '40998129-255c-4edf-7d7a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2dcba122-99e1-4474-4e7f-08d709bf1c3c', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', '3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('928c7718-d4af-437b-4e80-08d709bf1c3c', '3f70983f-1884-49ad-3640-08d709bf0b65', '7ec86556-becf-49f3-7d7b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('7d8eb0fb-70d0-4e45-4e81-08d709bf1c3c', 'c27b893b-796d-4bbf-3641-08d709bf0b65', '6330e0bd-42fc-4ac6-7d71-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b25b7a31-b47f-48e0-4e82-08d709bf1c3c', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '26b0f3a0-dd75-4e44-7d6f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('946d4682-3fcc-46e2-4e83-08d709bf1c3c', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '90678f43-39c7-4ccf-7d72-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('cadb4256-8dcc-4427-4e84-08d709bf1c3c', 'f41535d8-03ec-4005-3644-08d709bf0b65', '0df18f48-730c-4398-7d70-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('950793cf-c461-4da3-4e85-08d709bf1c3c', '5d542eef-5b8a-47b4-3645-08d709bf0b65', '22e89a74-3e03-4f1d-7d6b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('8b494bd4-dee1-438b-4e86-08d709bf1c3c', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 'a97d11cb-a691-4567-7d6c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e6edba8e-a8cd-4b5a-4e87-08d709bf1c3c', '6f9ae434-29bc-43b6-3647-08d709bf0b65', '0273a51b-6a4e-4b77-7d6a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('64c6e573-b134-41e6-4e88-08d709bf1c3c', '62663b74-a5d0-43ed-fa70-08d7072f517b', '598f8a41-6857-4012-7d69-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3718cd91-9120-4e01-4e89-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '75e133dc-63ae-4027-7d6e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4fe04787-cdc3-43d7-4e8a-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ecc7625d-2763-42aa-4e8b-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5241e0d6-0c82-4377-4e8c-08d709bf1c3c', 'f3e3bc58-3808-429c-3648-08d709bf0b65', '3e76b4cf-0ff3-4b75-7d61-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2471b4f9-dfe7-456b-4e8d-08d709bf1c3c', '0161d8ed-91de-48d2-3649-08d709bf0b65', '7d619e77-c7c4-446b-7d63-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('90e599e0-f8e3-4f8d-4e8f-08d709bf1c3c', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 'ff573676-00ce-46cf-7d62-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('f1889d5b-8ec3-402a-4e90-08d709bf1c3c', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5b755102-503b-43bc-4e91-08d709bf1c3c', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'b23935af-93a3-4eee-7d5b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('531d0c51-5000-4046-4e92-08d709bf1c3c', 'f9a8bd61-348e-4730-364b-08d709bf0b65', '308b4d8f-5db1-47e4-7d5d-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c527df14-8450-4156-4e93-08d709bf1c3c', '3d51d0dc-592d-4263-364c-08d709bf0b65', 'edfd819d-2424-446f-7d5f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ba3cb147-fb3e-42fa-4e94-08d709bf1c3c', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 'cb72e62b-60cb-4188-7d5e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b9a75303-660d-468b-4e95-08d709bf1c3c', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'ec48a0c4-ffed-43ad-7d5c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c60df8ff-eec9-4a8d-4e97-08d709bf1c3c', '29c3b92a-2252-42fc-3652-08d709bf0b65', '09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d17266d9-c47f-4893-4e98-08d709bf1c3c', 'f72a8122-cde0-4e65-3653-08d709bf0b65', '34a24768-5d7b-4ead-7d75-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e9f2ddf3-76fc-44e6-4e9a-08d709bf1c3c', 'f736892d-c4a7-4012-3651-08d709bf0b65', '09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('41fb7b3e-99a8-4893-4e9b-08d709bf1c3c', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('12456241-2644-4b13-4e9c-08d709bf1c3c', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '7a90f207-ec2a-435b-7d64-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d5450a3b-95e9-4673-4e9d-08d709bf1c3c', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', '3f047f21-0c05-4f78-7d66-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('94058c04-0681-4f46-4e9e-08d709bf1c3c', 'a93f70c4-61b9-481f-3655-08d709bf0b65', '1cbfc955-a4a4-48a2-7d67-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('0e6b4ff4-70b3-48d2-4e9f-08d709bf1c3c', '90617210-63ee-4d54-3656-08d709bf0b65', '5b3acd28-4d57-46ce-7d68-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3b13385b-1c4d-4de9-4ea0-08d709bf1c3c', '515f8064-e91c-41e2-fa74-08d7072f517b', 'b4d9c97c-a17d-48e5-7d65-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d801308e-504a-4cd9-b32e-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '5b343f0f-d587-4e10-7d77-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('85f867d5-a63d-42ec-b32f-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'b809dfc3-7af0-43c1-7d6d-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('123dff04-edc9-46a4-b330-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '598f8a41-6857-4012-7d69-08d708fe7bef')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('0973d470-ff1d-4e43-3619-08d7619a9701', '1ec0e308-6076-4759-ee2d-08d7618e356c', '101931d4-9689-4745-af0e-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('6a1d275e-9da7-4857-361a-08d7619a9701', 'ece2bc32-4a1b-4287-ee2e-08d7618e356c', '84db695c-de44-4629-af0d-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d7653813-8adf-4c45-361b-08d7619a9701', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', 'e0d35bdb-8e60-4f20-af0c-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('faac2564-99e5-4076-361c-08d7619a9701', 'b4829b5c-715c-44c3-ee2c-08d7618e356c', '4b58005c-07a9-4085-af0a-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d18f6267-c221-47d3-361d-08d7619a9701', 'b0c12bbd-cb1d-4125-fa70-08d7072f517b', 'd604eb0f-541b-4a65-af0b-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('670a01d6-c687-45e4-3645-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', '692c92d8-b178-47ab-aedc-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c829352f-c44a-4523-3646-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', '66545a16-d05a-4841-aee0-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ccc28514-d291-4eda-3647-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', '59f2e8e0-375a-4cd2-aee2-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('af514e6b-6d59-4afa-3648-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', 'a8e8700e-4373-4f13-aeeb-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c5019270-22d4-42b1-3649-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', 'be37712f-b5d3-4f57-aeec-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('a228c9c4-fa93-4f93-364a-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', 'aebaa69d-8115-444b-aeed-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('056180f4-d045-48ba-364b-08d7619a9701', '9a860fc5-05e1-471b-34c4-08d76192cd65', '25885f7d-8961-488f-aef4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('56bbffb6-e8b2-4710-364c-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'a95015fc-1704-4124-aed9-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('9f557cf8-a305-4d61-364d-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '2ce6bb53-4141-44c9-aeda-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b827f8b1-bdac-4bca-364e-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '5de7dfda-0ba1-4fbd-aedb-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e8c0f2ed-a973-41fa-364f-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'a8e8700e-4373-4f13-aeeb-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('089153a5-ae03-4083-3650-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'be37712f-b5d3-4f57-aeec-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e85c1bd1-7eff-4f35-3651-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'aebaa69d-8115-444b-aeed-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('95a2dbf9-4157-45e1-3652-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '2adb0cb9-fbb0-4749-aeee-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('9290fe43-e395-4dfb-3653-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'aaeaa10a-63ad-4617-aeef-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('173c7edb-0acd-40a4-3654-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'f86f43eb-1668-4e56-aef0-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ae6be4a3-6029-4459-3655-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '0461bb95-e517-4b95-aef1-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4999bc27-f598-4d72-3656-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '447d56cd-2c3a-4088-aef2-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('09fdd153-e7d0-46c7-3657-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', 'f46a5b73-dd16-40fd-aef3-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('7c1ecde8-7e57-4675-3658-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '25885f7d-8961-488f-aef4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('dff37ac9-7a76-445a-3659-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '3f1a6b0b-3978-4b22-aef5-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('701a0db3-b0d4-418d-365a-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '8f60955b-921a-4382-aef6-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d949aae0-8d65-4035-365b-08d7619a9701', '7d83ef7c-c41d-4819-34c5-08d76192cd65', '949a6e14-8a93-4e7d-aef7-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b8981509-2176-4795-365c-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'a95015fc-1704-4124-aed9-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('a7236f70-a7d4-4e45-365d-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '2ce6bb53-4141-44c9-aeda-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('693f73df-49d5-4ebe-365e-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '692c92d8-b178-47ab-aedc-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c7315c8d-9795-4335-365f-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'f44df964-6329-4096-aedd-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4df5f530-4505-429f-3660-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '29a3a368-b670-48ec-aede-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5325dc81-6e4e-4f4d-3661-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'a8d770f8-0e04-4a40-aedf-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('30c696af-f56e-4b76-3662-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'a0b29977-1ed6-4db4-aee1-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('fbcaa481-1fb2-41cb-3663-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '3cc758fa-4529-4b6f-aee3-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d05599cd-7560-434c-3664-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '5b78755a-fc36-47b2-aee4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4840ebd7-616a-4b76-3665-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'b0958d84-eaf7-44f0-aee5-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d85a7fe8-681e-4d07-3666-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '2700712d-7fd7-40ba-aee6-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('f933691f-856e-496e-3667-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'f0530bfd-eb45-476b-aee7-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('7f78c1f4-07d5-4889-3668-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'd730747d-e187-43e1-aee8-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4a7839e8-78a0-407d-3669-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '45df68d7-64a3-4c0e-aee9-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2f32a750-b95a-4de4-366a-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'f6bcf535-b52a-4477-aeea-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4ea2d0a9-96f1-4b18-366b-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'a8e8700e-4373-4f13-aeeb-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c74d765d-dd9c-4496-366c-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'be37712f-b5d3-4f57-aeec-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e59e8634-fc16-4520-366d-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', 'aebaa69d-8115-444b-aeed-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c5453760-0762-4c1f-366e-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '447d56cd-2c3a-4088-aef2-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e334870d-7b2b-42fb-366f-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '25885f7d-8961-488f-aef4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('872f9b54-1b73-40a3-3670-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '70717ba8-d99c-4993-af1a-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('6684f400-a469-409c-3671-08d7619a9701', '97c4e48b-159a-4a54-34c2-08d76192cd65', '5faf0ac9-8111-4589-af1b-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('bc3b1681-6485-4e9a-3672-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'a95015fc-1704-4124-aed9-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('6a82b4dd-793f-4c64-3673-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '2ce6bb53-4141-44c9-aeda-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('1424deb9-f75e-4924-3674-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '692c92d8-b178-47ab-aedc-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5a78e7b8-0cf4-48c8-3675-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'f44df964-6329-4096-aedd-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c2c449cb-a8f5-4edb-3676-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '29a3a368-b670-48ec-aede-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c1e2313d-04fd-40e7-3677-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'a8d770f8-0e04-4a40-aedf-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('98ae9ff0-786e-498b-3678-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '3cc758fa-4529-4b6f-aee3-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('fd68ca72-0845-45e2-3679-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '5b78755a-fc36-47b2-aee4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('215b3ae7-91aa-44b1-367a-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'b0958d84-eaf7-44f0-aee5-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4e657279-bb7b-4fd6-367b-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '2700712d-7fd7-40ba-aee6-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3cef3628-3741-4c31-367c-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'f0530bfd-eb45-476b-aee7-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('9e66a069-e174-4b0c-367d-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'd730747d-e187-43e1-aee8-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('492a63e3-2fa6-4d03-367e-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '45df68d7-64a3-4c0e-aee9-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('7f611291-223f-401f-367f-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'f6bcf535-b52a-4477-aeea-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3562bc52-bb0d-4bca-3680-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'a8e8700e-4373-4f13-aeeb-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5e41be9c-37c1-49af-3681-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'be37712f-b5d3-4f57-aeec-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('250cf6a1-3aea-4f6b-3682-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', 'aebaa69d-8115-444b-aeed-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('00ff797d-4858-44b8-3683-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '447d56cd-2c3a-4088-aef2-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('0dc21317-d395-4159-3684-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '25885f7d-8961-488f-aef4-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('04a012d7-5f6a-4823-3685-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '5faf0ac9-8111-4589-af1b-08d7619a4e89')");
            dbContext.Database.ExecuteSqlRaw("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2ee56fd6-c89f-42e4-3686-08d7619a9701', '1176960a-ea52-4dc2-34c3-08d76192cd65', '70717ba8-d99c-4993-af1a-08d7619a4e89')");
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
            //var removeApis = existApi.Except(apis, new ApiEquality());
            var addApis = apis.Except(existApi, new ApiEquality());
            //dbContext.Set<Api>().RemoveRange(removeApis);
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
