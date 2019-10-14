
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;
using Yu.Model.WorkFlow.WorkFlowDefine.OutputModels;

namespace Yu.Service.WorkFlow.WorkFlowDefines
{

    public interface IWorkFlowDefineService
    {
        /// <summary>
        /// 取得数据
        /// </summary>
        WorkFlowDefine GetWorkFlowById(Guid id);

        /// <summary>
        /// 取得数据
        /// </summary>
		PagedData<WorkflowDefineResult> GetWorkFlowDefines(int pageIndex, int pageSize, string typeId);

        /// <summary>
        /// 取得数据
        /// </summary>
		List<WorkflowDefineResult> GetAllWrokFlowDefines();

        /// <summary>
        /// 删除数据
        /// </summary>
        Task DeleteWorkFlowDefineAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddWorkFlowDefineAsync(WorkFlowDefine entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        Task UpdateWorkFlowDefineAsync(WorkFlowDefine entity);
    }
}

