using System;

namespace Yu.Model.WebAdmin.Role.InputOuputModels
{
    public class RoleDetail
    {
        public Guid Id { get; set; }

        // 角色名称
        public string Name { get; set; }

        // 描述
        public string Describe { get; set; }

        // 关联的页面元素
        public string[] Elements { get; set; }

        // 关联的数据规则
        public string[] DataRules { get; set; }
    }
}
