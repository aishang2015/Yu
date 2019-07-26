using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using Yu.Core.Extensions;

namespace Yu.Core.Jwt
{
    public static class JwtServiceCollectionExtension
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 使用缓存
            services.AddMemoryCache();

            // 注入Jwt配置文件
            services.Configure<JwtOption>(configuration.GetSection("JwtOption"));

            // 配置文件
            var option = services.BuildServiceProvider().GetRequiredService<IOptions<JwtOption>>().Value;

            services.AddAuthentication(ops =>
            {
                ops.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                ops.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // 握手认证
            }).AddJwtBearer(ops =>
            {
                // 验证参数
                ops.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = option.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = option.Audience,
                    ValidateAudience = true,
                    IssuerSigningKey = option.SecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                };

                // 自定义事件
                ops.Events = new JwtBearerEvents()
                {
                    OnMessageReceived = context => { return Task.CompletedTask; },
                    OnTokenValidated = context =>
                     {
                         // 取得当前请求下的memorycache
                         var memoryCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

                         // 判断用户名对应的token是否缓存
                         var userName = context.Principal.GetUserName();
                         var exist = memoryCache.TryGetValue(userName, out string token);
                         if (exist)
                         {
                             // 验证缓存的token和当前访问的token是否是同一个
                             var bearerToken = context.Request.Headers["Authorization"].ToString();
                             if (!bearerToken.Contains(token))
                             {
                                 context.Fail("TokenNoExist");
                             }
                         }
                         else
                         {
                             // 没有缓存的token 认证失败
                             context.Fail("TokenNoExist");
                         }
                         return Task.CompletedTask;
                     },
                    OnChallenge = context =>
                    {
                        if (context.AuthenticateFailure.Message == "TokenNoExist")
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 470;
                        }
                        return Task.CompletedTask;
                    }

                };
            });

            // 注入factory
            services.AddScoped<IJwtFactory, JwtFactory>();
        }
    }
}
