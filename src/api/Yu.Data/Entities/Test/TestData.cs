using System;
using System.ComponentModel;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Test
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    [DataRuleManage]
    [Description("测试实体")]
    public class TestData : BaseEntity<Guid>
    {
        [Description("字段1")]
        public string Field1 { get; set; }

        [Description("字段2")]
        public string Field2 { get; set; }

        [Description("字段3")]
        public string Field3 { get; set; }

        [Description("字段4")]
        public int Field4 { get; set; }

        [Description("创建的用户用户名")]
        public string UserName { get; set; }

        [Description("创建的用户组织ID")]
        public string UserGroupId { get; set; }
    }
}
