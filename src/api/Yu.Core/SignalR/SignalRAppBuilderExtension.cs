using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.SignalR
{
    public static class SignalRAppBuilderExtension
    {
        public static void UseSignalR(this IApplicationBuilder app)
        {
            app.UseSignalR(options =>
            {
                options.MapHub<SignalRHub>("/hub");
            });
        }
    }
}
