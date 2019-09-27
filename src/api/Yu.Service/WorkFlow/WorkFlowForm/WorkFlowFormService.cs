using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;

namespace Yu.Service.WorkFlow.WorkFlowForm
{
    public class WorkFlowFormService : IWorkFlowFormService
    {
        private IRepository<WorkFlowFormContent, Guid> _workflowFormContentRepository;

        private IRepository<WorkFlowFormElement, Guid> _workflowFormElementRepository;

        private IUnitOfWork<BaseIdentityDbContext> _unitOfWork;


        public WorkFlowFormService(
            IRepository<WorkFlowFormContent, Guid> workflowFormContentRepository,
            IRepository<WorkFlowFormElement, Guid> workflowFormElementRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _workflowFormContentRepository = workflowFormContentRepository;
            _workflowFormElementRepository = workflowFormElementRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加或更新工作流表单
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <param name="content">表单内容</param>
        /// <param name="elements">表单元素</param>
        public async Task AddOrUpdateWorkFlowFormAsync(Guid defineId, WorkFlowFormContent content, IEnumerable<WorkFlowFormElement> elements)
        {
            _workflowFormContentRepository.DeleteRange(wffc => wffc.DefineId == defineId);
            await _workflowFormContentRepository.InsertAsync(content);
            _workflowFormElementRepository.DeleteRange(wffe => wffe.DefineId == defineId);
            await _workflowFormElementRepository.InsertRangeAsync(elements);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得表单内容
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <returns></returns>
        public WorkFlowFormContent GetWorkFlowFormContent(Guid defineId)
        {
            return _workflowFormContentRepository.GetByWhereNoTracking(wffc => wffc.DefineId == defineId).FirstOrDefault();
        }

        /// <summary>
        /// 取得表单元素
        /// </summary>
        /// <param name="defineId">工作流id</param>
        /// <returns></returns>
        public IEnumerable<WorkFlowFormElement> GetWorkFlowFormElements(Guid defineId)
        {
            return _workflowFormElementRepository.GetByWhereNoTracking(wffe => wffe.DefineId == defineId);
        }

        /// <summary>
        /// 移除工作流表单
        /// </summary>
        /// <param name="defineId">表单id</param>
        public async Task RemoveWorkFlowFormAsync(Guid defineId)
        {
            _workflowFormContentRepository.DeleteRange(wffc => wffc.DefineId == defineId);
            _workflowFormElementRepository.DeleteRange(wffe => wffe.DefineId == defineId);
            await _unitOfWork.CommitAsync();
        }
    }
}
