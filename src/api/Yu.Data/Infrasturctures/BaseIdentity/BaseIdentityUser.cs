using Microsoft.AspNetCore.Identity;
using System;
using Yu.Data.Entities.Enums;

namespace Yu.Data.Infrasturctures.BaseIdentity
{
    public partial class BaseIdentityUser : IdentityUser<Guid>
    {
        #region 认证字段

        // 微信OpenId
        public string OpenId { get; set; }

        // 微信昵称
        public string NickName { get; set; }

        #endregion

        #region 用户信息字段

        // 姓名
        public string FullName { get; set; }

        // 头像
        public string Avatar { get; set; }

        // 性别
        public Gender Gender { get; set; }

        // 出生日期
        public DateTime? Birthday { get; set; }

        // 籍贯
        public string Native { get; set; }

        // 毕业学校
        public string Graduate { get; set; }

        // 学历
        public string Education { get; set; }

        #endregion

        #region 权限管理模块字段
        
        public string UserGroupId { get; set; }

        public string GroupName { get; set; }

        public string Roles { get; set; }

        #endregion
    }
}
