using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities.Right;
using Yu.Model.WebAdmin.Rule.InputModels;
using Yu.Model.WebAdmin.Rule.OutputModels;
using Yu.Service.WebAdmin.Rule;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("规则管理")]
    public class RuleController : AnonymousController
    {

        private readonly IRuleService _ruleService;

        public RuleController(IRuleService ruleService)
        {
            _ruleService = ruleService;
        }

        [HttpGet("ruleGroup")]
        [Description("查看所有规则组")]
        public IActionResult GetAllRuleGroup()
        {
            var groups = _ruleService.GetAllRuleGroup();
            return Ok(groups);
        }

        [HttpGet("ruleDetail")]
        [Description("查看规则组内容")]
        public IActionResult GetGroupDetail([FromQuery]Guid ruleGroupId)
        {
            var detail = _ruleService.GetRuleResult(ruleGroupId);
            return Ok(detail);
        }

        [HttpDelete("ruleGroup")]
        [Description("删除规则组")]
        public async Task<IActionResult> DeleteRuleGroup([FromQuery]Guid ruleGroupId)
        {
            await _ruleService.DeleteRuleGroup(ruleGroupId);
            return Ok();
        }

        [HttpPut("ruleDetail")]
        [Description("添加修改规则组")]
        //public async Task<IActionResult> AddOrUpdateRule([FromBody]IEnumerable<Rule> rules,
        //    [FromBody]IEnumerable<RuleCondition> ruleConditions, [FromBody]RuleGroup ruleGroup)
        public async Task<IActionResult> AddOrUpdateRule([FromBody]RuleResult result)
        {
            await _ruleService.AddOrUpdateRule(result.Rules, result.RuleConditions, result.RuleGroup);
            return Ok();
        }

    }
}