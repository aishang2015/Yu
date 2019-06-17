using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.Account.OutputModels
{
    public class LoginResult
    {
        // jwtToken
        public string Token { get; set; }

        // 用户名
        public string UserName { get; set; }

        // 头像地址
        public string AvatarUrl { get; set; }
    }
}
