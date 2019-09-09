using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using System;
using Yu.Core.AutoMapper;
using Yu.Core.Captcha;
using Yu.Core.Cors;
using Yu.Core.Extensions;
using Yu.Core.FileManage;
using Yu.Core.Jwt;
using Yu.Core.MQTT;
using Yu.Core.Quartznet;
using Yu.Core.SignalR;
using Yu.Core.Swagger;
using Yu.Core.Validators;
using Yu.Data.Infrasturctures;
using Yu.Data.MongoDB;
using Yu.Data.Redis;

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
            services.AddCaptcha(Configuration); // 配置验证码工具

            services.AddAutoMapper();   // 配置AutoMapper

            services.AddCustomCors(Configuration);  // 配置自定义跨域策略

            services.AddIdentityDbContext<BaseIdentityDbContext, BaseIdentityUser, BaseIdentityRole, Guid>
                (Configuration.GetConnectionString("SqlServerConnection1"), DatabaseType.SqlServer); // 认证数据库上下文

            services.AddJwtAuthentication(Configuration, true); // 配置jwt认证

            services.AddScopedBatch("Yu.Service.dll"); // 批量注入service

            services.AddMvc(ops =>
            {
                ops.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, name) => $"值'{value}'不是合法的'{name}'(SYSTEM)");
            }).AddFluentValidators(); // 添加fluentvalidation支持

            services.ConfigureFluentValidationModelErrors(); // 统一模型验证结果

            services.AddSwaggerConfiguration(); // 配置swagger

            services.AddFileManage(); // 静态文件操作类

            services.AddQuartzNet(); // 添加作业调度

            services.AddDistributedRedisCache(Configuration); // 使用Redis分布式缓存

            services.AddMongoDb(Configuration); // 添加mongodb支持

            services.AddMqtt(Configuration); // 添加MQTT支持

            services.AddSignalR(); // 添加signalr
        }

        // 构建管道
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            LogManager.Configuration.Install(new InstallationContext());
            if (env.IsDevelopment())
            {
                app.UseDevelopExceptionHandler();
            }
            else
            {
                app.UseHsts(); // 严格http传输
            }

            app.UseMqtt(); // 解析mqtt请求。
                       
            app.UseCustomCors();    // 使用自定义跨域策略

            app.SeedIdentityDbData<BaseIdentityDbContext>();   // 初始化BaseIdentityDbContext数据

            app.UseAuthentication(); // 使用认证策略

            app.UseSignalR(); // 使用signalr

            app.UseStaticFiles(Configuration, "AvatarFileOption"); // 配置静态文件访问路径和服务器目录

            app.UserSwaggerConfiguration(); // 使用Swagger

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}