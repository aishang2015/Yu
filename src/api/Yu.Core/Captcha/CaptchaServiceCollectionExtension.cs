using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Captcha
{
    public static class CaptchaServiceCollectionExtension
    {
        // 注入配置文件和工具类
        public static void AddCaptcha(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<CaptchaOption>(configuration.GetSection("CaptchaOption"));
            services.AddSingleton<CaptchaHelper>();
        }
    }
}
