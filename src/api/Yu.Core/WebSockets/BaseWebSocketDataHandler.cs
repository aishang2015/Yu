using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yu.Core.WebSockets
{
    public class BaseWebSocketDataHandler : IWebSocketDataHandler
    {
        public Task HandleData(WebSocketData webSocketData, byte[] bytes)
        {
            return Task.CompletedTask;
        }

        public Task SendToOne(WebSocketData webSocketData, string data)
        {
            return Task.CompletedTask;
        }

        public Task SendToSome(IEnumerable<WebSocketData> webSocketDatas, string data)
        {
            return Task.CompletedTask;
        }
    }
}
