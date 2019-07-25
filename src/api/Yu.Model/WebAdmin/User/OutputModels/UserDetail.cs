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

        // 用户姓名
        public virtual string FullName { get; set; }

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

        // 出生日期
        public string Birthday { get; set; }

        // 籍贯
        public string Native { get; set; }

        // 毕业学校
        public string Graduate { get; set; }

        // 学历
        public string Education { get; set; }

        // 用户角色
        public string[] Roles { get; set; }

        // 组织Id
        public string GroupId { get; set; }

        // 组织名称
        public string GroupName { get; set; }
    }
}
