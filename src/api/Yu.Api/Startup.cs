using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            services.AddSession(ops =>
            {
                ops.IdleTimeout = TimeSpan.FromMinutes(5);
            });

            services.AddCaptcha(Configuration); // 配置验证码工具

            services.AddAutoMapper();   // 配置AutoMapper

            services.AddCustomCors(Configuration);  // 配置自定义跨域策略

            services.AddIdentityDbContext(Configuration.GetConnectionString("SqlServerConnection"), DatabaseType.SqlServe); // 认证数据库上下文

            services.AddRepositories(); // 批量注入Yu.Data内实体的仓储实现

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

            app.SeedDbData();   // 初始化数据

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
