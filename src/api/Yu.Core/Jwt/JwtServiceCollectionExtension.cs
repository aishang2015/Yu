using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace Yu.Core.Jwt
{
    public static class JwtServiceCollectionExtension
    {
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 注入Jwt配置文件
            services.Configure<JwtOption>(configuration.GetSection("JwtOption"));

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
                    OnMessageReceived = context => { return Task.CompletedTask; }
                };
            });
        }
    }
}
