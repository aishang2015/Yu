
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;
 
namespace Yu.Service.WorkFlow.WorkFlowFlowNodes
{

	public interface IWorkFlowFlowNodeService
	{
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
        /// 提交数据
        /// </summary>
        Task CommitAllAsync();

    }
}

