
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;
 
namespace Yu.Service.WorkFlow.WorkFlowInstances
{

	public interface IWorkFlowInstanceService
	{
        /// <summary>
        /// 取得数据
        /// </summary>
		PagedData<WorkFlowInstance> GetWorkFlowInstances(int pageIndex, int pageSize, string searchText);
		
        /// <summary>
        /// 删除数据
        /// </summary>
        Task DeleteWorkFlowInstanceAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddWorkFlowInstanceAsync(WorkFlowInstance entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        Task UpdateWorkFlowInstanceAsync(WorkFlowInstance entity);
	}
}

