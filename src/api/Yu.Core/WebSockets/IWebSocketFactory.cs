using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Core.WebSockets
{
    public interface IWebSocketFactory
    {
        // 根据id查找websocket
        WebSocketData Get(Guid webSocketId);

        // 添加
        void Add(WebSocketData webSocketData);

        // 删除
        void Remove(Guid webSocketId);

        // 获取列表
        IEnumerable<WebSocketData> All();

    }
}
