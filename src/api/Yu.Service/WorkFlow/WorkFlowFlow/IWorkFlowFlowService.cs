using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Service.WorkFlow.WorkFlowFlow
{
    public interface IWorkFlowFlowService
    {

        /// <summary>
        /// 取得数据
        /// </summary>
        List<WorkFlowFlowConnection> GetWorkFlowFlowConnections(Guid id);

        /// <summary>
        /// 删除数据
        /// </summary>
        void DeleteWorkFlowFlowConnectionAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddWorkFlowFlowConnectionsAsync(List<WorkFlowFlowConnection> entities);

        /// <summary>
        /// 取得数据
        /// </summary>
		List<WorkFlowFlowNode> GetWorkFlowFlowNodes(Guid id);

        /// <summary>
        /// 删除数据
        /// </summary>
        void DeleteWorkFlowFlowNodeAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddWorkFlowFlowNodesAsync(List<WorkFlowFlowNode> entities);

        /// <summary>
        /// 保存流程
        /// </summary>
        Task AddOrUpdateFlow(Guid defineId, List<WorkFlowFlowNode> nodes, List<WorkFlowFlowConnection> connections);
    }
}
