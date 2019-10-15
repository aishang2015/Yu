
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities;
using Yu.Model.Common.InputModels;
using Yu.Data.Entities.WorkFlow;
using Yu.Service.WorkFlow.WorkFlowInstances;
using Yu.Core.Extensions;

namespace Yu.Api.Areas.WorkFlow.Controller
{
    [Route("api")]
    [Description("工作流实例")]
    public class WorkFlowInstanceController : AuthorizeController
    {
        private readonly IWorkFlowInstanceService _service;

        public WorkFlowInstanceController(IWorkFlowInstanceService service)
        {
            _service = service;
        }

		/// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("workflowInstance")]
        [Description("取得工作流实例数据")]
        public IActionResult GetWorkFlowInstances([FromQuery] PagedQuery query)
        {
            var result = _service.GetWorkFlowInstances(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        [HttpPost("workflowInstance")]
        [Description("添加工作流实例数据")]
        public async Task<IActionResult> AddWorkFlowInstance([FromBody]WorkFlowInstance entity)
        {
            entity.UserName = User.GetUserName();            
            await _service.AddWorkFlowInstanceAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        [HttpPut("workflowInstance")]
        [Description("更新工作流实例数据")]
        public async Task<IActionResult> UpdateWorkFlowInstance([FromBody]WorkFlowInstance entity)
        {
            await _service.UpdateWorkFlowInstanceAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        [HttpDelete("workflowInstance")]
        [Description("删除工作流实例数据")]
        public async Task<IActionResult> DeleteWorkFlowInstance([FromQuery]IdQuery query)
        {
            await _service.DeleteWorkFlowInstanceAsync(query.Id);
            return Ok();
        }
	}
}

