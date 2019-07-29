using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;
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
        public IActionResult GetGroupDetail([FromQuery]IdQuery query)
        {
            var detail = _ruleService.GetRuleResult(query.Id);
            return Ok(detail);
        }

        [HttpDelete("ruleGroup")]
        [Description("删除规则组")]
        public async Task<IActionResult> DeleteRuleGroup([FromQuery]IdQuery query)
        {
            await _ruleService.DeleteRuleGroupAsync(query.Id);
            return Ok();
        }

        [HttpPut("ruleDetail")]
        [Description("添加修改规则组")]
        //public async Task<IActionResult> AddOrUpdateRule([FromBody]IEnumerable<Rule> rules,
        //    [FromBody]IEnumerable<RuleCondition> ruleConditions, [FromBody]RuleGroup ruleGroup)
        public async Task<IActionResult> AddOrUpdateRule([FromBody]RuleResult result)
        {
            var taskResult = await _ruleService.AddOrUpdateRuleAsync(result.Rules, result.RuleConditions, result.RuleGroup);
            if (!taskResult)
            {
                ModelState.AddModelError("field", ErrorMessages.WebAdmin_Rule_E001);
                return BadRequest(ModelState);
            }
            return Ok();
        }

    }
}