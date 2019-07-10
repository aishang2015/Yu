using System;
using Yu.Core.Expressions;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class RuleCondition : BaseEntity<Guid>
    {
        // 所属规则ID
        public Guid RuleId { get; set; }

        // 所属组织
        public Guid RuleGroupId { get; set; }

        // 字段
        public string Field { get; set; }

        // 操作方式
        public ExpressionType OperateType { get; set; }

        // 值
        public string Value { get; set; }
    }
}
