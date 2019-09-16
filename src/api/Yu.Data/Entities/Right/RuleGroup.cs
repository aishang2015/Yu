using System;
using System.ComponentModel;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    [Description("规则组")]
    public class RuleGroup : BaseEntity<Guid>
    {
        [Description("规则组名称")]
        public string Name { get; set; }

        [Description("数据库上下文")]
        public string DbContext { get; set; }

        [Description("对应实体")]
        public string Entity { get; set; }
    }
}
