using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WebAdmin.Rule.OutputModels
{
    public class RuleEntityResult
    {

        public string Id { get; set; }

        /// <summary>
        /// 规则组ID
        /// </summary>
        public string RuleGroupId { get; set; }

        /// <summary>
        /// 上级规则
        /// </summary>
        public string UpRuleId { get; set; }

        /// <summary>
        /// 表达式组合方式
        /// </summary>
        public string CombineType { get; set; }
    }
}
