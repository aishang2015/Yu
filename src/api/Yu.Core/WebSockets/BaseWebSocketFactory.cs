using System;
using System.Collections.Generic;
using System.Linq;

namespace Yu.Core.WebSockets
{
    public class BaseWebSocketFactory : IWebSocketFactory
    {
        private static List<WebSocketData> _webSockets = new List<WebSocketData>();

        public void Add(WebSocketData webSocketData)
        {
            _webSockets.Add(webSocketData);
        }

        public IEnumerable<WebSocketData> All()
        {
            return _webSockets;
        }

        public WebSocketData Get(Guid webSocketId)
        {
            return _webSockets.FirstOrDefault(w => w.WebSocketId == webSocketId);
        }

        public void Remove(Guid webSocketId)
        {
            _webSockets.Remove(Get(webSocketId));
        }
    }
}
