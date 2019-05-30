using Microsoft.AspNetCore.Builder;
using System;

namespace Yu.Core.WebSockets
{
    public static class WebSocketsAppBuilderExtension
    {
        public static void UseWebSocketManager(this IApplicationBuilder app)
        {
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.UseMiddleware<WebSocketManagerMiddleware>();
        }
    }
}
