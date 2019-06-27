using System.Collections.Generic;
using Yu.Data.Entities.Right;
using RuleEntity = Yu.Data.Entities.Right.Rule;

namespace Yu.Model.WebAdmin.Rule.InputModels
{
    public class RuleDetail
    {
        public IEnumerable<RuleEntity> Rules { get; set; }

        public IEnumerable<RuleCondition> RuleConditions { get; set; }

        public RuleGroup RuleGroup { get; set; }
    }
}
