using System;
using System.Net.WebSockets;

namespace Yu.Core.WebSockets
{
    public class WebSocketData
    {
        public WebSocket WebSocket { get; set; }

        public Guid WebSocketId { get; set; }
    }
}
