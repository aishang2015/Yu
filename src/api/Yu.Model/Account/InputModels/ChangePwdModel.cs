using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.Account.InputModels
{
    public class ChangePwdModel
    {

        /// <summary>
        /// 旧密码
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }
    }
}
