using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace Yu.Core.Swagger
{
    public static class SwaggerServiceCollectionExtension
    {
        // 容器注入配置
        public static void AddSwaggerConfiguration(this IServiceCollection services)
        {
            // 定义文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });
        }

    }
}
