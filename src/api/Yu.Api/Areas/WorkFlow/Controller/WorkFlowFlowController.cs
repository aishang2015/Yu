
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.WorkFlow.WorkFlowFlowNodes.InputModels;
using Yu.Model.WorkFlow.WorkFlowFlow;
using Yu.Service.WorkFlow.WorkFlowFlow;
using Yu.Data.Infrasturctures.BaseIdentity.Mvc;

namespace Yu.Api.Areas.WorkFlow.Controllers
{
    [Route("api")]
    [Description("工作流流程节点管理")]
    public class WorkFlowFlowController : ApiAuthorizeController
    {
        private readonly IWorkFlowFlowService _workFlowFlowService;

        public WorkFlowFlowController(IWorkFlowFlowService workFlowFlowService)
        {
            _workFlowFlowService = workFlowFlowService;
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("workflowFlow")]
        [Description("取得工作流流程节点和连接")]
        public IActionResult GetWorkFlowFlowNodes([FromQuery] WorkFlowFlowQuery query)
        {
            var nodes = _workFlowFlowService.GetWorkFlowFlowNodes(query.DefineId);
            var connections = _workFlowFlowService.GetWorkFlowFlowConnections(query.DefineId);
            var nodeElements = _workFlowFlowService.GetWorkFlowFlowNodeElements(query.DefineId);

            return Ok(new WorkFlowFlowViewModel
            {
                Nodes = nodes,
                Connections = connections,
                NodeElements = nodeElements,
                DefineId = query.DefineId
            });
        }

        /// <summary>
        /// 创建更新数据
        /// </summary>
        [HttpPut("workflowFlow")]
        [Description("更新工作流流程节点和连接")]
        public async Task<IActionResult> AddOrUpdateWorkFlowFlowNode([FromBody]WorkFlowFlowViewModel model)
        {
            await _workFlowFlowService.AddOrUpdateFlow(model.DefineId, model.Nodes, model.Connections, model.NodeElements);
            return Ok();
        }
    }
}

