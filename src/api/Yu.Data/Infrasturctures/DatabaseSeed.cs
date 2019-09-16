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
                    FullName = xing[random.Next(0, xing.Count)] + xing[random.Next(0, xing.Count)]
                };
                userManager.CreateAsync(user, CommonConstants.Password).Wait();
                userManager.AddToRoleAsync(user, "系统管理员").Wait();
            }
        }

        // 初始化其他数据
        private static void InitOtherData<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('9807863c-b234-4452-7d57-08d708fe7bef',N'账户管理-用户名密码登陆','Account','POST','/api/Account/Login')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('47317471-a6d6-4c6f-7d58-08d708fe7bef',N'账户管理-刷新token','Account','POST','/api/Account/RefreshToken')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('8b02d6f4-c6e7-44ea-7d59-08d708fe7bef',N'账户管理-修改密码','Account','POST','/api/Account/ChangePwd')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('4768aa73-11a1-4016-7d5a-08d708fe7bef',N'验证码-获取验证码','Captcha','GET','/api/Captcha')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('b23935af-93a3-4eee-7d5b-08d708fe7bef',N'接口管理-取得全部Api数据','Api','GET','/api/allApi')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('ec48a0c4-ffed-43ad-7d5c-08d708fe7bef',N'接口管理-取得API数据','Api','GET','/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('308b4d8f-5db1-47e4-7d5d-08d708fe7bef',N'接口管理-添加API数据','Api','POST','/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('cb72e62b-60cb-4188-7d5e-08d708fe7bef',N'接口管理-更新api数据','Api','PUT','/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('edfd819d-2424-446f-7d5f-08d708fe7bef',N'接口管理-删除api数据','Api','DELETE','/api/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('fa75a6b9-743e-4961-7d60-08d708fe7bef',N'页面元素管理-取得页面元素','Element','GET','/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('3e76b4cf-0ff3-4b75-7d61-08d708fe7bef',N'页面元素管理-添加新元素','Element','POST','/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('ff573676-00ce-46cf-7d62-08d708fe7bef',N'页面元素管理-删除元素','Element','DELETE','/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('7d619e77-c7c4-446b-7d63-08d708fe7bef',N'页面元素管理-更新元素','Element','PUT','/api/element')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('7a90f207-ec2a-435b-7d64-08d708fe7bef',N'实体管理-取得实体数据(下拉框用)','Entity','GET','/api/entities')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('b4d9c97c-a17d-48e5-7d65-08d708fe7bef',N'实体管理-取得实体数据','Entity','GET','/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('3f047f21-0c05-4f78-7d66-08d708fe7bef',N'实体管理-添加实体数据','Entity','POST','/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('1cbfc955-a4a4-48a2-7d67-08d708fe7bef',N'实体管理-更新实体数据','Entity','PUT','/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('5b3acd28-4d57-46ce-7d68-08d708fe7bef',N'实体管理-删除实体数据','Entity','DELETE','/api/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('598f8a41-6857-4012-7d69-08d708fe7bef',N'组织结构管理-取得所有组织','Group','GET','/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('0273a51b-6a4e-4b77-7d6a-08d708fe7bef',N'组织结构管理-删除组织','Group','DELETE','/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('22e89a74-3e03-4f1d-7d6b-08d708fe7bef',N'组织结构管理-创建组织','Group','POST','/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('a97d11cb-a691-4567-7d6c-08d708fe7bef',N'组织结构管理-更新组织','Group','PUT','/api/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('b809dfc3-7af0-43c1-7d6d-08d708fe7bef',N'角色管理-取得全部角色名称','Role','GET','/api/roleNames')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('75e133dc-63ae-4027-7d6e-08d708fe7bef',N'角色管理-取得角色概要数据','Role','GET','/api/roleOutline')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('26b0f3a0-dd75-4e44-7d6f-08d708fe7bef',N'角色管理-取得角色详细数据','Role','GET','/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('0df18f48-730c-4398-7d70-08d708fe7bef',N'角色管理-删除角色数据','Role','DELETE','/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('6330e0bd-42fc-4ac6-7d71-08d708fe7bef',N'角色管理-创建角色','Role','POST','/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('90678f43-39c7-4ccf-7d72-08d708fe7bef',N'角色管理-更新角色','Role','PUT','/api/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('8488942f-ed09-4d55-7d73-08d708fe7bef',N'规则管理-查看所有规则组','Rule','GET','/api/ruleGroup')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('a6b958f7-5d93-456c-7d74-08d708fe7bef',N'规则管理-查看规则组内容','Rule','GET','/api/ruleDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('34a24768-5d7b-4ead-7d75-08d708fe7bef',N'规则管理-删除规则组','Rule','DELETE','/api/ruleGroup')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('09861d8a-2d36-4d6f-7d76-08d708fe7bef',N'规则管理-添加修改规则组','Rule','PUT','/api/ruleDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('5b343f0f-d587-4e10-7d77-08d708fe7bef',N'用户管理-取得用户概要情报','User','GET','/api/userOutline')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('3220f72a-a4ca-4326-7d78-08d708fe7bef',N'用户管理-取得用户数据','User','GET','/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('afb4c264-b57a-4936-7d79-08d708fe7bef',N'用户管理-添加新用户','User','POST','/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('40998129-255c-4edf-7d7a-08d708fe7bef',N'用户管理-更新用户数据','User','PUT','/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('7ec86556-becf-49f3-7d7b-08d708fe7bef',N'用户管理-删除用户数据','User','DELETE','/api/userDetail')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Api\"(\"Id\",\"Name\",\"Controller\",\"Type\",\"Address\")VALUES('65f688c1-c679-4d38-7d7c-08d708fe7bef',N'用户管理-设置用户头像','User','POST','api/userAvatar')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('c5fdd1a3-aafe-430e-fa6c-08d7072f517b',N'权限管理',1,'rightmanage','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('ad592b36-dd5b-447b-fa6d-08d7072f517b',N'用户管理',1,'usermanage','/right/user')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('7bfaa83d-8611-4fcd-fa6e-08d7072f517b',N'角色管理',1,'rolemanage','/right/role')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('62663b74-a5d0-43ed-fa70-08d7072f517b',N'组织管理',1,'groupmanage','/right/group')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('5b859edf-eac7-46a7-fa71-08d7072f517b',N'页面元素管理',1,'elementmanage','/right/menu')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('b08a4c30-d492-4537-fa72-08d7072f517b',N'api数据管理',1,'apimanage','/right/api')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('e68887e2-1f67-4ee3-fa73-08d7072f517b',N'规则管理',1,'rulemanage','/right/rule')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('515f8064-e91c-41e2-fa74-08d7072f517b',N'实体数据管理',1,'entitymanage','/right/entity')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('96469b51-7236-48c4-363c-08d709bf0b65',N'添加用户',2,'adduserbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('c1be82cf-cf72-425f-363d-08d709bf0b65',N'查看用户',3,'viewuserlink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('88877f44-13f8-41c5-363e-08d709bf0b65',N'编辑头像',3,'editavatarlink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('b3440e7f-5aa1-424a-363f-08d709bf0b65',N'编辑信息',3,'edituserlink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('3f70983f-1884-49ad-3640-08d709bf0b65',N'删除用户',3,'deleteuserlink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('c27b893b-796d-4bbf-3641-08d709bf0b65',N'添加角色',2,'addrolebtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('7c3e6882-22c3-45e7-3643-08d709bf0b65',N'修改角色',3,'editrolelink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('f41535d8-03ec-4005-3644-08d709bf0b65',N'删除角色',3,'deleterolelink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('5d542eef-5b8a-47b4-3645-08d709bf0b65',N'创建组织',2,'addgroupbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('a0e99d34-8dc2-4fe1-3646-08d709bf0b65',N'修改组织',2,'editgroupbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('6f9ae434-29bc-43b6-3647-08d709bf0b65',N'删除组织',2,'deletegroupbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('f3e3bc58-3808-429c-3648-08d709bf0b65',N'添加元素',2,'addelementbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('0161d8ed-91de-48d2-3649-08d709bf0b65',N'修改元素',2,'editelementbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('fbcb28e3-36b3-4227-364a-08d709bf0b65',N'删除元素',2,'deleteelementbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('f9a8bd61-348e-4730-364b-08d709bf0b65',N'添加api',2,'addapibtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('3d51d0dc-592d-4263-364c-08d709bf0b65',N'删除api',3,'deleteapilink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('6ce2bc0b-951e-4818-364d-08d709bf0b65',N'修改api',3,'editapilink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('f736892d-c4a7-4012-3651-08d709bf0b65',N'添加规则',2,'addrulegroupbtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('29c3b92a-2252-42fc-3652-08d709bf0b65',N'编辑规则',3,'editrulegroup','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('f72a8122-cde0-4e65-3653-08d709bf0b65',N'删除规则',3,'deleterulegrouplink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('e6e3efe1-10f0-4078-3654-08d709bf0b65',N'添加数据',2,'addentitybtn','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('a93f70c4-61b9-481f-3655-08d709bf0b65',N'修改数据',3,'editentitylink','')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO  \"Element\"(\"Id\",\"Name\",\"ElementType\",\"Identification\",\"Route\")VALUES('90617210-63ee-4d54-3656-08d709bf0b65',N'删除数据',3,'deletedatalink','')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('260632bd-6db9-4d83-4e7b-08d709bf1c3c', '96469b51-7236-48c4-363c-08d709bf0b65', 'afb4c264-b57a-4936-7d79-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3ca2b96a-3d90-4730-4e7c-08d709bf1c3c', 'c1be82cf-cf72-425f-363d-08d709bf0b65', '3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('9fab1859-d95c-43d8-4e7d-08d709bf1c3c', '88877f44-13f8-41c5-363e-08d709bf0b65', '65f688c1-c679-4d38-7d7c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c5695b43-fa18-465b-4e7e-08d709bf1c3c', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', '40998129-255c-4edf-7d7a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2dcba122-99e1-4474-4e7f-08d709bf1c3c', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', '3220f72a-a4ca-4326-7d78-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('928c7718-d4af-437b-4e80-08d709bf1c3c', '3f70983f-1884-49ad-3640-08d709bf0b65', '7ec86556-becf-49f3-7d7b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('7d8eb0fb-70d0-4e45-4e81-08d709bf1c3c', 'c27b893b-796d-4bbf-3641-08d709bf0b65', '6330e0bd-42fc-4ac6-7d71-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b25b7a31-b47f-48e0-4e82-08d709bf1c3c', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '26b0f3a0-dd75-4e44-7d6f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('946d4682-3fcc-46e2-4e83-08d709bf1c3c', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '90678f43-39c7-4ccf-7d72-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('cadb4256-8dcc-4427-4e84-08d709bf1c3c', 'f41535d8-03ec-4005-3644-08d709bf0b65', '0df18f48-730c-4398-7d70-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('950793cf-c461-4da3-4e85-08d709bf1c3c', '5d542eef-5b8a-47b4-3645-08d709bf0b65', '22e89a74-3e03-4f1d-7d6b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('8b494bd4-dee1-438b-4e86-08d709bf1c3c', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 'a97d11cb-a691-4567-7d6c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e6edba8e-a8cd-4b5a-4e87-08d709bf1c3c', '6f9ae434-29bc-43b6-3647-08d709bf0b65', '0273a51b-6a4e-4b77-7d6a-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('64c6e573-b134-41e6-4e88-08d709bf1c3c', '62663b74-a5d0-43ed-fa70-08d7072f517b', '598f8a41-6857-4012-7d69-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3718cd91-9120-4e01-4e89-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '75e133dc-63ae-4027-7d6e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('4fe04787-cdc3-43d7-4e8a-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ecc7625d-2763-42aa-4e8b-08d709bf1c3c', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5241e0d6-0c82-4377-4e8c-08d709bf1c3c', 'f3e3bc58-3808-429c-3648-08d709bf0b65', '3e76b4cf-0ff3-4b75-7d61-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('2471b4f9-dfe7-456b-4e8d-08d709bf1c3c', '0161d8ed-91de-48d2-3649-08d709bf0b65', '7d619e77-c7c4-446b-7d63-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('90e599e0-f8e3-4f8d-4e8f-08d709bf1c3c', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 'ff573676-00ce-46cf-7d62-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('f1889d5b-8ec3-402a-4e90-08d709bf1c3c', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'fa75a6b9-743e-4961-7d60-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('5b755102-503b-43bc-4e91-08d709bf1c3c', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'b23935af-93a3-4eee-7d5b-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('531d0c51-5000-4046-4e92-08d709bf1c3c', 'f9a8bd61-348e-4730-364b-08d709bf0b65', '308b4d8f-5db1-47e4-7d5d-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c527df14-8450-4156-4e93-08d709bf1c3c', '3d51d0dc-592d-4263-364c-08d709bf0b65', 'edfd819d-2424-446f-7d5f-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('ba3cb147-fb3e-42fa-4e94-08d709bf1c3c', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 'cb72e62b-60cb-4188-7d5e-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('b9a75303-660d-468b-4e95-08d709bf1c3c', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'ec48a0c4-ffed-43ad-7d5c-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('c60df8ff-eec9-4a8d-4e97-08d709bf1c3c', '29c3b92a-2252-42fc-3652-08d709bf0b65', '09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d17266d9-c47f-4893-4e98-08d709bf1c3c', 'f72a8122-cde0-4e65-3653-08d709bf0b65', '34a24768-5d7b-4ead-7d75-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('e9f2ddf3-76fc-44e6-4e9a-08d709bf1c3c', 'f736892d-c4a7-4012-3651-08d709bf0b65', '09861d8a-2d36-4d6f-7d76-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('41fb7b3e-99a8-4893-4e9b-08d709bf1c3c', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '8488942f-ed09-4d55-7d73-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('12456241-2644-4b13-4e9c-08d709bf1c3c', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '7a90f207-ec2a-435b-7d64-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d5450a3b-95e9-4673-4e9d-08d709bf1c3c', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', '3f047f21-0c05-4f78-7d66-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('94058c04-0681-4f46-4e9e-08d709bf1c3c', 'a93f70c4-61b9-481f-3655-08d709bf0b65', '1cbfc955-a4a4-48a2-7d67-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('0e6b4ff4-70b3-48d2-4e9f-08d709bf1c3c', '90617210-63ee-4d54-3656-08d709bf0b65', '5b3acd28-4d57-46ce-7d68-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('3b13385b-1c4d-4de9-4ea0-08d709bf1c3c', '515f8064-e91c-41e2-fa74-08d7072f517b', 'b4d9c97c-a17d-48e5-7d65-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('d801308e-504a-4cd9-b32e-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '5b343f0f-d587-4e10-7d77-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('85f867d5-a63d-42ec-b32f-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'b809dfc3-7af0-43c1-7d6d-08d708fe7bef')");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementApi\" (\"Id\", \"ElementId\", \"ApiId\") VALUES ('123dff04-edc9-46a4-b330-08d71efcd6ad', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '598f8a41-6857-4012-7d69-08d708fe7bef')");

            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('05e877a1-d897-4a68-6d14-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b66e1bef-da5d-4ad1-6d15-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('2b83f45d-0b18-44fb-6d16-08d7072f517f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('cdbaf940-e8ae-4fe9-6d17-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('80f5ef02-0277-4d95-6d18-08d7072f517f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d059c25b-ba9f-4ac0-6d1c-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '62663b74-a5d0-43ed-fa70-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('29fdbb81-ed7d-4f22-6d1d-08d7072f517f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '62663b74-a5d0-43ed-fa70-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('39d441b8-9a2b-47dd-6d1e-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '5b859edf-eac7-46a7-fa71-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c8ae766b-e077-4e7b-6d1f-08d7072f517f', '5b859edf-eac7-46a7-fa71-08d7072f517b', '5b859edf-eac7-46a7-fa71-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f6a9a41f-59d9-4b07-6d20-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b08a4c30-d492-4537-fa72-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('4274a4dd-9660-4906-6d21-08d7072f517f', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'b08a4c30-d492-4537-fa72-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f37856a6-6221-4630-6d22-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('41c5a517-75a4-4834-6d23-08d7072f517f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('69253c30-9f02-4fc9-6d24-08d7072f517f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '515f8064-e91c-41e2-fa74-08d7072f517b', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('487f5dd0-65a0-4365-6d25-08d7072f517f', '515f8064-e91c-41e2-fa74-08d7072f517b', '515f8064-e91c-41e2-fa74-08d7072f517b', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d425521c-60ba-4d2d-a94d-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '96469b51-7236-48c4-363c-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('87bdeded-2ee7-4597-a94e-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '96469b51-7236-48c4-363c-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c3bbfcd2-f730-4602-a94f-08d709bf0b6f', '96469b51-7236-48c4-363c-08d709bf0b65', '96469b51-7236-48c4-363c-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e29b8667-292b-42ce-a950-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('0a866110-9f55-4b51-a951-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('455b67bc-e2be-4b63-a952-08d709bf0b6f', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 'c1be82cf-cf72-425f-363d-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c2972e6c-63ee-4fcf-a953-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '88877f44-13f8-41c5-363e-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('5482fb60-7240-48d0-a954-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '88877f44-13f8-41c5-363e-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('23bb2e14-5b04-46e1-a955-08d709bf0b6f', '88877f44-13f8-41c5-363e-08d709bf0b65', '88877f44-13f8-41c5-363e-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('83833d08-fa3c-46d2-a956-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('974d3101-6579-45ee-a957-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d729c043-2626-43ac-a958-08d709bf0b6f', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 'b3440e7f-5aa1-424a-363f-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9b83a5f5-4e0f-45aa-a959-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '3f70983f-1884-49ad-3640-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('65a8a5bc-7af3-47dc-a95a-08d709bf0b6f', 'ad592b36-dd5b-447b-fa6d-08d7072f517b', '3f70983f-1884-49ad-3640-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8cf18883-822b-4932-a95b-08d709bf0b6f', '3f70983f-1884-49ad-3640-08d709bf0b65', '3f70983f-1884-49ad-3640-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3b6c8e81-ec3a-4f4b-a95c-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6605567b-c8b7-4aee-a95d-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('531141eb-c5dd-424b-a95e-08d709bf0b6f', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 'c27b893b-796d-4bbf-3641-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('09b1cf87-cf84-4b38-a962-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7bb16f1e-216b-4932-a963-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6c2e601f-0ebc-4656-a964-08d709bf0b6f', '7c3e6882-22c3-45e7-3643-08d709bf0b65', '7c3e6882-22c3-45e7-3643-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('941b16c5-6f9c-409a-a965-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f41535d8-03ec-4005-3644-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c07d5b17-2001-4e2f-a966-08d709bf0b6f', '7bfaa83d-8611-4fcd-fa6e-08d7072f517b', 'f41535d8-03ec-4005-3644-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('80f6eabd-5b15-4320-a967-08d709bf0b6f', 'f41535d8-03ec-4005-3644-08d709bf0b65', 'f41535d8-03ec-4005-3644-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('82f153c7-36db-497c-a968-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('120852ef-7ea3-4008-a969-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('89402946-0c4e-402d-a96a-08d709bf0b6f', '5d542eef-5b8a-47b4-3645-08d709bf0b65', '5d542eef-5b8a-47b4-3645-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('346e9d28-09ab-4bb3-a96b-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7afdc9ec-ab50-4f7a-a96c-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('fd90f5ae-0207-4da2-a96d-08d709bf0b6f', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 'a0e99d34-8dc2-4fe1-3646-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3fc5db84-aa00-4939-a96e-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1d44302b-5f9b-42ff-a96f-08d709bf0b6f', '62663b74-a5d0-43ed-fa70-08d7072f517b', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('a7427846-7e3a-4e9d-a970-08d709bf0b6f', '6f9ae434-29bc-43b6-3647-08d709bf0b65', '6f9ae434-29bc-43b6-3647-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('88b1b3ba-4b2a-42e5-a971-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b8092fb7-3a59-44a8-a972-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dbb4d014-5fe9-4475-a973-08d709bf0b6f', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 'f3e3bc58-3808-429c-3648-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1ca9e923-04a9-4f85-a974-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '0161d8ed-91de-48d2-3649-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('2ab6fb14-b3a3-4035-a975-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', '0161d8ed-91de-48d2-3649-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('392a406f-b17c-4fe9-a976-08d709bf0b6f', '0161d8ed-91de-48d2-3649-08d709bf0b65', '0161d8ed-91de-48d2-3649-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9b84c68c-af3d-41f5-a977-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('deee1c1a-4747-484b-a978-08d709bf0b6f', '5b859edf-eac7-46a7-fa71-08d7072f517b', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('c0824065-945b-41f7-a979-08d709bf0b6f', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 'fbcb28e3-36b3-4227-364a-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('225723a8-705c-45c6-a97a-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6d307c00-b49d-456d-a97b-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8a694ade-afab-4d8c-a97c-08d709bf0b6f', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 'f9a8bd61-348e-4730-364b-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e655b031-c778-41b7-a97d-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '3d51d0dc-592d-4263-364c-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('0e4f1199-5bcd-4e08-a97e-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', '3d51d0dc-592d-4263-364c-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('56594a89-2f5c-417a-a97f-08d709bf0b6f', '3d51d0dc-592d-4263-364c-08d709bf0b65', '3d51d0dc-592d-4263-364c-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('688ff1e0-6409-42ec-a980-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('9a8204d8-0733-42ac-a981-08d709bf0b6f', 'b08a4c30-d492-4537-fa72-08d7072f517b', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d51841ed-f853-4d1f-a982-08d709bf0b6f', '6ce2bc0b-951e-4818-364d-08d709bf0b65', '6ce2bc0b-951e-4818-364d-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dee0d080-202c-4d1e-a983-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f736892d-c4a7-4012-3651-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('57c31bee-6973-4d89-a984-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'f736892d-c4a7-4012-3651-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('1ecff558-6a3a-4b72-a985-08d709bf0b6f', 'f736892d-c4a7-4012-3651-08d709bf0b65', 'f736892d-c4a7-4012-3651-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ad5fae0d-1388-46b4-a986-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '29c3b92a-2252-42fc-3652-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('8739e373-4d70-433d-a987-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', '29c3b92a-2252-42fc-3652-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('3d5c2071-3360-4a48-a988-08d709bf0b6f', '29c3b92a-2252-42fc-3652-08d709bf0b65', '29c3b92a-2252-42fc-3652-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('d2a63141-b34e-4609-a989-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('b2200994-cbad-4c0b-a98a-08d709bf0b6f', 'e68887e2-1f67-4ee3-fa73-08d7072f517b', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('6b2f9ed9-833c-4bf5-a98b-08d709bf0b6f', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 'f72a8122-cde0-4e65-3653-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f1ebe3fe-7cdd-4d79-a98c-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('7dcbabbe-b41c-4b02-a98d-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('5ab5227a-a6cb-4a3f-a98e-08d709bf0b6f', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 'e6e3efe1-10f0-4078-3654-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('61f5001a-d6cc-4055-a98f-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('05f08ea7-e6ab-446f-a990-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('dd7eac0b-7f14-4769-a991-08d709bf0b6f', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 'a93f70c4-61b9-481f-3655-08d709bf0b65', 0)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('e54fdc52-d0a9-475d-a992-08d709bf0b6f', 'c5fdd1a3-aafe-430e-fa6c-08d7072f517b', '90617210-63ee-4d54-3656-08d709bf0b65', 2)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('ae2a0b11-1fa2-4932-a993-08d709bf0b6f', '515f8064-e91c-41e2-fa74-08d7072f517b', '90617210-63ee-4d54-3656-08d709bf0b65', 1)");
            dbContext.Database.ExecuteSqlCommand("INSERT INTO \"ElementTree\" (\"Id\", \"Ancestor\", \"Descendant\", \"Length\") VALUES ('f888d3cf-de89-4ed1-a994-08d709bf0b6f', '90617210-63ee-4d54-3656-08d709bf0b65', '90617210-63ee-4d54-3656-08d709bf0b65', 0)");

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
