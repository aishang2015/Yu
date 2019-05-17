using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yu.Core.Captcha;
using Yu.Data.Entities;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession(ops =>
            {
                ops.IdleTimeout = TimeSpan.FromMinutes(5);
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddCaptcha(Configuration); // 配置登录验证码工具

            services.AddIdentityDbContext(Configuration.GetConnectionString("SqlServerConnection"), DatabaseType.SqlServe); // 认证数据库上下文

            services.AddRepositories(); // 批量注入Yu.Data内实体的仓储实现

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseSession(); // 使用Session

            app.SeedDbData();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
