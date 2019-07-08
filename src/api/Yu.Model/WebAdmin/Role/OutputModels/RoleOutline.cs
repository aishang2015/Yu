using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.Role.OutputModels
{
    public class RoleOutline
    {
        public Guid Id { get; set; }

        // 角色名称
        public string Name { get; set; }

        // 描述
        public string Describe { get; set; }
    }
}
