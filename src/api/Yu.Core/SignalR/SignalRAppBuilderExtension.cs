using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.SignalR
{
    public static class SignalRAppBuilderExtension
    {
        public static void UseSignalR(this IApplicationBuilder app)
        {
            app.UseEndpoints(options =>
            {
                options.MapHub<SignalRHub>("/hub");
            });
        }

        public static void UseSignalR<T>(this IApplicationBuilder app, string pattern) where T : Hub
        {
            app.UseEndpoints(options =>
            {
                options.MapHub<T>(pattern);
            });

        }
    }
}
