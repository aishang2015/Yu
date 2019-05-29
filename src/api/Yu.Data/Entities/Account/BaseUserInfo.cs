using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.Enums;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Account
{
    // 用户基本数据
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class BaseUserInfo : BaseEntity<Guid>
    {
        // 昵称
        public string NickName { get; set; }

        // 头像
        public string Avatar { get; set; }

        // 性别
        public GenderEnum Gender { get; set; }
    }
}
