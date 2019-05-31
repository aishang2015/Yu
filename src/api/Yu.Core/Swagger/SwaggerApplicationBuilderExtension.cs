using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.Swagger
{
    public static  class SwaggerApplicationBuilderExtension
    {
        // 使用swagger
        public static void UserSwaggerConfiguration(this IApplicationBuilder app)
        {
            app.UseSwagger();

            // 浏览
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
