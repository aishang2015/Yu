using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Yu.Core.FileManage
{
    public static class StaticFileApplicationBuilderExtension
    {
        public static void UseStaticFiles(this IApplicationBuilder app, IConfiguration configuration)
        {

            // 设置文件的物理存储路径和请求访问路径
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(configuration["StaticFileOptions:ServerFileStorePath"]),
                RequestPath = configuration["StaticFileOptions:RequestPath"]
            });
        }
    }
}
