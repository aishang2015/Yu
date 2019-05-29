using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Yu.Core.Extensions
{
    public static class ExceptionHandlerExtension
    {
        // 开发环境
        public static void UseDevelopExceptionHandler(this IApplicationBuilder app)
        {
            // 捕获异常，设置httpcode 为500
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
                    }
                });
            });
        }

        // 生产环境
        public static void UseProductionExceptionHandler(this IApplicationBuilder app)
        {
            // 捕获异常，设置httpcode 为500
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await context.Response.WriteAsync("系统故障").ConfigureAwait(false);
                });
            });
        }
    }
}
