using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.IO;

namespace Yu.Core.FileManage
{
    public static class StaticFileApplicationBuilderExtension
    {
        public static void UseStaticFiles(this IApplicationBuilder app, IConfiguration configuration, string group)
        {
            var serverFileStorePath = configuration[$"{group}:ServerFileStorePath"];
            if (!Directory.Exists(serverFileStorePath))
            {
                Directory.CreateDirectory(serverFileStorePath);
            }

            // 设置文件的物理存储路径和请求访问路径
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(serverFileStorePath),
                RequestPath = configuration[$"{group}:RequestPath"]
            });
        }
    }
}
