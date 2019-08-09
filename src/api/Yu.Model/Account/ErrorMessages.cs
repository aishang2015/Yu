using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.Message
{
    public partial class ErrorMessages
    {
        public static readonly string Account_E001= "用户名不能为空!";
        public static readonly string Account_E002 = "密码不能为空!";
        public static readonly string Account_E003 = "请输入验证码!";

        public static readonly string Account_E004 = "验证码已过期，请重新获取！";
        public static readonly string Account_E005 = "验证码不正确!";
        public static readonly string Account_E006 = "用户名或密码不正确!";

        public static readonly string Account_E007 = "Token刷新失败!";

        public static readonly string Account_E008 = "密码修改失败!请确认旧密码是否正确，新密码强度是否足够。";
        public static readonly string Account_E009 = "找不到此用户，请确认手机号码是否正确。";
        public static readonly string Account_E010 = "密码重置失败，请确认新密码强度足够或验证码在有效期内。";
    }
}
