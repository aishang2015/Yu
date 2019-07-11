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
        /// 用户角色
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// 组织名称
        /// </summary>
        public string GroupName { get; set; }
    }
}
