using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Service.WorkFlow.WorkFlowForm
{
    public interface IWorkFlowFormService
    {
        /// <summary>
        /// 取得表单内容
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <returns></returns>
        WorkFlowFormContent GetWorkFlowFormContent(Guid defineId);

        /// <summary>
        /// 取得表单元素
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <returns></returns>
        IEnumerable<WorkFlowFormElement> GetWorkFlowFormElements(Guid defineId);

        /// <summary>
        /// 添加或更新工作流表单
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <param name="content">表单内容</param>
        /// <param name="elements">表单元素</param>
        Task AddOrUpdateWorkFlowFormAsync(Guid defineId, WorkFlowFormContent content, IEnumerable<WorkFlowFormElement> elements);

        /// <summary>
        /// 移除工作流表单
        /// </summary>
        /// <param name="defineId">表单id</param>
        Task RemoveWorkFlowFormAsync(Guid defineId);
    }
}
