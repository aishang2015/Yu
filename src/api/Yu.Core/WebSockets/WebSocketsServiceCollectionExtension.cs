using Microsoft.Extensions.DependencyInjection;

namespace Yu.Core.WebSockets
{
    // 配置websocket需要的实例
    public static class WebSocketsServiceCollectionExtension
    {

        // 添加handler,factory
        public static void AddWebSocketManager<TWebSocketDataHandler, TWebSocketFactory>(this IServiceCollection services)
            where TWebSocketDataHandler : class, IWebSocketDataHandler
            where TWebSocketFactory : class, IWebSocketFactory
        {
            services.AddSingleton<IWebSocketDataHandler, TWebSocketDataHandler>();
            services.AddSingleton<IWebSocketFactory, TWebSocketFactory>();
        }

    }
}
