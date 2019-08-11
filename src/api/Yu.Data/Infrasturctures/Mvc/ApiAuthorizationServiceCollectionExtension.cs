using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Yu.Data.Infrasturctures.Mvc
{
    public static class ApiAuthorizationServiceCollectionExtension
    {
        public static void AddApiAuthorization(this IServiceCollection services)
        {
            //【授权】
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiPermission", policy => policy.Requirements.Add(new ApiAuthorizationRequirement()));
            });

            // 注入权限处理器
            services.AddTransient<IAuthorizationHandler, ApiAuthorizationHandler>();
        }
    }
}