using Microsoft.AspNetCore.Identity;
using System;
using Yu.Data.Entities.Enums;

namespace Yu.Data.Infrasturctures
{
    public partial class BaseIdentityUser : IdentityUser<Guid>
    {
        #region 认证字段

        public string OpenId { get; set; }

        #endregion


        #region 用户信息字段

        // 昵称
        public string NickName { get; set; }

        // 头像
        public string Avatar { get; set; }

        // 性别
        public Gender Gender { get; set; }

        #endregion

        #region 权限管理冗余字段

        public string Roles { get; set; }

        public string GroupName { get; set; }

        public Guid GroupId { get; set; }

        #endregion
    }
}
