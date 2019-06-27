using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Model.WebAdmin.Rule.InputModels;
using Yu.Service.WebAdmin.Rule;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("规则管理")]
    public class RuleController : ControllerBase
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
        public async Task<IActionResult> AddOrUpdateRule([FromBody]RuleDetail ruleDetail)
        {
            await _ruleService.AddOrUpdateRule(ruleDetail.Rules, ruleDetail.RuleConditions, ruleDetail.RuleGroup);
            return Ok();
        }

    }
}