using Microsoft.AspNetCore.Identity;
using System;
using Yu.Data.Entities.Enums;

namespace Yu.Data.Infrasturctures
{
    public partial class BaseIdentityUser : IdentityUser<Guid>
    {
        // username，email，phonenumber，openid等为认证用字段
        public string OpenId { get; set; }


        #region 用户信息字段

        // 昵称
        public string NickName { get; set; }

        // 头像
        public string Avatar { get; set; }

        // 性别
        public Gender Gender { get; set; }

        #endregion
    }
}
