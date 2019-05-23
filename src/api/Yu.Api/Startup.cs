using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using System;
using Yu.Core.AutoMapper;
using Yu.Core.Captcha;
using Yu.Core.Cors;
using Yu.Core.Validators;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;

namespace Yu.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // 配置服务
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddFluentValidators();  // 配置fluentvalidation

            services.AddSession(ops => ops.IdleTimeout = TimeSpan.FromMinutes(5)); // 设置session

            services.AddCaptcha(Configuration); // 配置验证码工具

            services.AddAutoMapper();   // 配置AutoMapper

            services.AddCustomCors(Configuration);  // 配置自定义跨域策略

            services.AddIdentityDbContext<BaseIdentityDbContext, BaseIdentityUser, BaseIdentityRole, Guid>
                (Configuration.GetConnectionString("SqlServerConnection1"), DatabaseType.SqlServer); // 认证数据库上下文

            services.AddRepositories<BaseIdentityDbContext>(); // 批量注入仓储

            services.AddCommonDbContext<BaseDbContext>
                (Configuration.GetConnectionString("SqlServerConnection2"), DatabaseType.SqlServer);

            services.AddRepositories<BaseDbContext>(); // 批量注入仓储


        }

        // 构建管道
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            LogManager.Configuration.Install(new InstallationContext());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSession();   // 使用Session

            app.UseCustomCors();    // 使用自定义跨域策略

            app.SeedDbData<BaseIdentityDbContext>(context =>
            {
                var user = new BaseIdentityUser
                {
                    UserName = "admin",
                };
                user.PasswordHash = new PasswordHasher<BaseIdentityUser>().HashPassword(user, "P@ssword1");
                context.Set<BaseIdentityUser>().Add(user);
                context.SaveChanges();
            });   // 初始化BaseIdentityDbContext数据

            app.SeedDbData<BaseDbContext>(context => { }); // 初始化BaseDbContext数据

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
