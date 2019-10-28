
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;
using Yu.Model.WorkFlow.WorkFlowInstance.OutputModels;

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
        /// 更新或保存表单值
        /// </summary>
        Task AddOrUpdateWorkFlowInstanceForm(Guid instanceId, List<WorkFlowInstanceForm> forms);

        /// <summary>
        /// 取得工作流实例表单值
        /// </summary>
        List<WorkFlowInstanceForm> GetWorkFlowInstanceForm(Guid instanceId);

        /// <summary>
        /// 取得工作流实例节点处理数据
        /// </summary>
        List<WorkFlowInstanceNodeResult> GetWorkFlowInstanceNode(Guid instanceId);

        /// <summary>
        /// 更新数据
        /// </summary>
        Task UpdateWorkFlowInstanceAsync(WorkFlowInstance entity);

        /// <summary>
        /// 取得被删除的工作流实例
        /// </summary>
        PagedData<WorkFlowInstance> GetDeletedWorkFlowInstanceForm(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 设置工作流实例删除位
        /// </summary>
        Task<bool> SetWorkFlowInstanceDelete(Guid id, bool isDelete);

        /// <summary>
        /// 控制工作流实例状态
        /// </summary>
        Task HandleWorkFlowInstance(Guid instanceId, int handleStatus, string explain);
    }
}

