using System.Collections.Generic;
using Yu.Data.Entities.Right;
using RuleEntity = Yu.Data.Entities.Right.Rule;

namespace Yu.Model.WebAdmin.Rule.OutputModels
{
    public class RuleResult
    {
        public IEnumerable<RuleEntityResult> Rules { get; set; }

        public IEnumerable<RuleConditionResult> RuleConditions { get; set; }

        public RuleGroup RuleGroup { get; set; }
    }
}
