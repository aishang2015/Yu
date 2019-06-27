using System;
using Yu.Core.Expressions;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Rule : BaseEntity<Guid>
    {
        /// <summary>
        /// 规则组ID
        /// </summary>
        public Guid RuleGroupId { get; set; }

        /// <summary>
        /// 上级规则
        /// </summary>
        public Guid UpRuleId { get; set; }

        /// <summary>
        /// 表达式组合方式
        /// </summary>
        public ExpressionCombineType CombineType { get; set; }
    }
}
