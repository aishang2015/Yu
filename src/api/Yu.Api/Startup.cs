using Microsoft.AspNetCore.Authorization;
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
using Yu.Core.Swagger;
using Yu.Core.Validators;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Service.Handler;

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
            services.AddMemoryCache(); // 添加缓存

            services.AddCaptcha(Configuration); // 配置验证码工具

            services.AddAutoMapper();   // 配置AutoMapper

            services.AddCustomCors(Configuration);  // 配置自定义跨域策略

            services.AddIdentityDbContext<BaseIdentityDbContext, BaseIdentityUser, BaseIdentityRole, Guid>
                (Configuration.GetConnectionString("SqlServerConnection1"), DatabaseType.SqlServer); // 认证数据库上下文

            services.AddJwtAuthentication(Configuration); // 配置jwt认证

            services.AddApiAuthorization(); // 添加api认证Handler

            services.AddRepositories<BaseIdentityDbContext>(); // 批量注入仓储

            services.AddCommonDbContext<BaseDbContext>
                (Configuration.GetConnectionString("SqlServerConnection2"), DatabaseType.SqlServer); // 添加多个数据库

            services.AddRepositories<BaseDbContext>(); // 批量注入仓储

            services.AddScopedBatch("Yu.Service.dll"); // 批量注入service

            services.AddMvc(ops =>
            {
                ops.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, name) => $"值'{value}'不是合法的'{name}'(SYSTEM)");
            }).AddFluentValidators(); // 添加fluentvalidation支持

            services.ConfigureFluentValidationModelErrors(); // 统一模型验证结果的一致性

            services.AddSwaggerConfiguration(); // 配置swagger

            services.AddFileManage(); // 静态文件操作类

            //【授权】
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiPermission", policy => policy.Requirements.Add(new ApiAuthorizationRequirement()));
            });
            // 注入权限处理器
            services.AddTransient<IAuthorizationHandler, ApiAuthorizationHandler>();
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

            app.UseCustomCors();    // 使用自定义跨域策略

            app.SeedIdentityDbData<BaseIdentityDbContext>();   // 初始化BaseIdentityDbContext数据

            app.SeedDbData<BaseDbContext>(context => { }); // 初始化BaseDbContext数据

            app.UseAuthentication(); // 使用认证策略

            app.UseStaticFiles(Configuration, "AvatarFileOption"); // 配置静态文件访问路径和服务器目录

            app.UserSwaggerConfiguration(); // 使用Swagger

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
