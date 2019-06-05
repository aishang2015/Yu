using System;
using Yu.Data.Entities.Enums;

namespace Yu.Model.WebAdmin.User.OutputModels
{
    public class UserDetail
    {
        // ID
        public Guid Id { get; set; }

        // 用户名
        public virtual string UserName { get; set; }

        // 邮箱
        public virtual string Email { get; set; }

        // 手机号码
        public virtual string PhoneNumber { get; set; }

        // OpenId
        public string OpenId { get; set; }

        // 昵称
        public string NickName { get; set; }

        // 头像
        public string Avatar { get; set; }

        // 性别
        public Gender Gender { get; set; }
    }
}
