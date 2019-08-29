using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Yu.Core.SignalR
{
    [Authorize]
    public class SignalRHub : Hub
    {
        /// <summary>
        /// 用户连接
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            var user = Context.User;
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// 用户断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = Context.User;
            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 全体客户端发布消息
        /// </summary>
        /// <param name="message"></param>
        public async void SendToAllClient(string message)
        {
            await Clients.All.SendAsync("SendToAllClient", Context.UserIdentifier, message);
        }

        /// <summary>
        /// 发送到群组
        /// </summary>
        /// <param name="message"></param>
        /// <param name="groupName"></param>
        public async void SendToGroup(string message, string groupName)
        {
            await Clients.Group(groupName).SendAsync("SendToGroup",message);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task GetInfo(string info)
        {
            await Clients.All.SendAsync("test",  "hello");
            await Task.CompletedTask;
        }

    }
}
