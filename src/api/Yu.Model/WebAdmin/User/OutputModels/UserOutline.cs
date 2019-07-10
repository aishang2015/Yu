using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.User.OutputModels
{
    public class UserOutline
    {
        /// <summary>
        /// ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号	
        /// </summary>
        public string TelePhone { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 微信编号
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public string[] Roles { get; set; }
    }
}
