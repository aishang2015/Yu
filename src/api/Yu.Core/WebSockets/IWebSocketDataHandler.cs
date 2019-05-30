using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yu.Core.WebSockets
{
    public interface IWebSocketDataHandler
    {
        // 发送给某个客户端
        Task SendToOne(WebSocketData webSocketData, string data);

        // 发送给指定客户端
        Task SendToSome(IEnumerable<WebSocketData> webSocketDatas, string data);

        // 解析数据
        Task HandleData(WebSocketData webSocketData, byte[] bytes);
    }
}
