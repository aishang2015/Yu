using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Captcha
{
    public static class CaptchaExtension
    {
        public static void AddCaptcha(this IServiceCollection services,IConfiguration configuration)
        {
            services.Configure<CaptchaOption>(configuration.GetSection("CaptchaOption"));
            services.AddSingleton<CaptchaHelper>();
        }
    }
}
