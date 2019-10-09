using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.WorkFlow.WorkFlowForm;
using Yu.Service.WorkFlow.WorkFlowForm;

namespace Yu.Api.Areas.WorkFlow.Controller
{
    [Route("api")]
    [Description("工作流表单管理")]
    public class WorkFlowFormController : AuthorizeController
    {
        private readonly IWorkFlowFormService _workFlowFormService;

        public WorkFlowFormController(IWorkFlowFormService workFlowFormService)
        {
            _workFlowFormService = workFlowFormService;
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("workflowForm")]
        [Description("取得工作流表单数据")]
        public IActionResult GetWorkFlowForm([FromQuery]IdQuery query)
        {
            var formContent = _workFlowFormService.GetWorkFlowFormContent(query.Id);
            var formElements = _workFlowFormService.GetWorkFlowFormElements(query.Id);
            return Ok(new WorkFlowFormViewModel
            {
                DefineId = query.Id,
                FormContent = formContent,
                FormElements = formElements.ToList()
            });
        }

        /// <summary>
        /// 取得元素数据
        /// </summary>
        [HttpGet("workflowFormElement")]
        [Description("取得工作流表单元素数据")]
        public IActionResult GetWorkFlowFormElement([FromQuery]IdQuery query)
        {
            var formElements = _workFlowFormService.GetWorkFlowFormElements(query.Id);
            return Ok(formElements);
        }

        [HttpPut("workflowForm")]
        [Description("更新添加表单内容")]
        public async Task<IActionResult> AddOrUpdateWorkFlowForm([FromBody]WorkFlowFormViewModel model)
        {
            await _workFlowFormService.AddOrUpdateWorkFlowFormAsync(model.DefineId, model.FormContent, model.FormElements);
            return Ok();
        }
    }
}