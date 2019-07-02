using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.Rule.OutputModels
{
    public class RuleConditionResult
    {
        public string Id { get; set; }

        // 所属规则ID
        public string RuleId { get; set; }

        // 所属组织
        public string RuleGroupId { get; set; }

        // 字段
        public string Field { get; set; }

        // 操作方式
        public string OperateType { get; set; }

        // 值
        public string Value { get; set; }
    }
}
