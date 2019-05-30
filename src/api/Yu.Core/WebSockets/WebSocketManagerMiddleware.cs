using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Yu.Core.WebSockets
{
    public class WebSocketManagerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IWebSocketFactory _webSocketFactory;

        private readonly IWebSocketDataHandler _webSocketDataHandler;

        public WebSocketManagerMiddleware(RequestDelegate next,
            IWebSocketFactory webSocketFactory,
            IWebSocketDataHandler webSocketDataHandler)
        {
            _next = next;
            _webSocketFactory = webSocketFactory;
            _webSocketDataHandler = webSocketDataHandler;
        }


        /// <summary>
        /// 自定义中间件
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            // 判断wesocket连接的路径
            if (context.Request.Path == "/ws")
            {
                // 判断类型
                if (context.WebSockets.IsWebSocketRequest)
                {
                    // 开始连接
                    var socket = await context.WebSockets.AcceptWebSocketAsync();

                    // WebSocketData对象
                    var webSocketData = new WebSocketData()
                    {
                        WebSocket = socket,
                        WebSocketId = Guid.NewGuid()
                    };
                    _webSocketFactory.Add(webSocketData);

                    // 开始监听
                    await Listening(webSocketData);
                }
            }

            await _next(context);
        }

        // 监听程序
        private async Task Listening(WebSocketData webSocketData)
        {
            // 数据缓冲区
            var buffer = new byte[1024 * 4];

            // 取得websocket
            var webSocket = webSocketData.WebSocket;

            // 循环读取数据数据
            var result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            while (!result.CloseStatus.HasValue)
            {
                await _webSocketDataHandler.HandleData(webSocketData, buffer);
                buffer = new byte[1024 * 4];
                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            }

            // 关闭连接
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

            // 删除存储
            _webSocketFactory.Remove(webSocketData.WebSocketId);
        }
    }
}
