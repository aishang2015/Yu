
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;
 
namespace Yu.Service.WorkFlow.WorkFlowFlowConnections
{

	public interface IWorkFlowFlowConnectionService
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
	}
}

