using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.Account.InputModels
{
    /// <summary>
    /// 登录模型
    /// </summary>
    public class LoginModel
    {       
        /// <summary>            
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string CaptchaCode { get; set; }

    }
}
