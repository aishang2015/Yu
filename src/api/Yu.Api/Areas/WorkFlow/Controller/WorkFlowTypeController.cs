
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities;
using Yu.Data.Entities.Right;
using Yu.Model.Common.InputModels;
using Yu.Data.Entities.WorkFlow;
using Yu.Service.WorkFlow.WorkFlowTypes;
using Yu.Model.Message;

namespace Yu.Api.Areas.WorkFlow.Controllers
{
    [Route("api")]
    [Description("工作流管理")]
    public class WorkFlowTypeController : AuthorizeController
    {
        private readonly IWorkFlowTypeService _service;

        public WorkFlowTypeController(IWorkFlowTypeService service)
        {
            _service = service;
        }

		/// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("workflowType")]
        [Description("取得工作流类型数据")]
        public IActionResult GetWorkFlowTypes()
        {
            var result = _service.GetWorkFlowTypes();
            return Ok(result);
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        [HttpPost("workflowType")]
        [Description("添加工作流类型数据")]
        public async Task<IActionResult> AddWorkFlowType([FromBody]WorkFlowType entity)
        {
            var result = _service.HaveRepeatName(entity.Id, entity.Name);
            if (result)
            {
                ModelState.AddModelError("Name", ErrorMessages.WorkFlow_Type_E001);
                return BadRequest(ModelState);
            }

            await _service.AddWorkFlowTypeAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        [HttpPut("workflowType")]
        [Description("更新工作流类型数据")]
        public async Task<IActionResult> UpdateWorkFlowType([FromBody]WorkFlowType entity)
        {
            var result = _service.HaveRepeatName(entity.Id, entity.Name);
            if (result)
            {
                ModelState.AddModelError("Name", ErrorMessages.WorkFlow_Type_E001);
                return BadRequest(ModelState);
            }

            await _service.UpdateWorkFlowTypeAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        [HttpDelete("workflowType")]
        [Description("删除工作流类型数据")]
        public async Task<IActionResult> DeleteWorkFlowType([FromQuery]IdQuery query)
        {
            var result = _service.HaveWorkFlowDefine(query.Id);
            if (result)
            {
                ModelState.AddModelError("Name", ErrorMessages.WorkFlow_Type_E002);
                return BadRequest(ModelState);
            }

            await _service.DeleteWorkFlowTypeAsync(query.Id);
            return Ok();
        }
	}
}

