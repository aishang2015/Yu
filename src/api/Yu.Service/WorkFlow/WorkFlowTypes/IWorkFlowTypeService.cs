
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Service.WorkFlow.WorkFlowTypes
{

    public interface IWorkFlowTypeService
    {
        /// <summary>
        /// 取得数据
        /// </summary>
		List<WorkFlowType> GetWorkFlowTypes();

        /// <summary>
        /// 删除数据
        /// </summary>
        Task DeleteWorkFlowTypeAsync(Guid id);

        /// <summary>
        /// 添加数据
        /// </summary>
        Task AddWorkFlowTypeAsync(WorkFlowType entity);

        /// <summary>
        /// 更新数据
        /// </summary>
        Task UpdateWorkFlowTypeAsync(WorkFlowType entity);

        /// <summary>
        /// 检查名称是否重复
        /// </summary>
        bool HaveRepeatName(Guid id, string name);

        /// <summary>
        /// 检查类型下是否定义了流程
        /// </summary>
        bool HaveWorkFlowDefine(Guid id);

        /// <summary>
        /// 获取类型名称
        /// </summary>
        string GetTypeNameById(Guid id);
    }
}

