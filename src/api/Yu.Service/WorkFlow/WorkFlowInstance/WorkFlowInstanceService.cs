
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Service.WorkFlow.WorkFlowInstances
{
    public class WorkFlowInstanceService : IWorkFlowInstanceService
    {

        // 仓储类
        private IRepository<WorkFlowInstance, Guid> _repository;

        // 工作流表单值仓储
        private IRepository<WorkFlowInstanceForm, Guid> _workflowInstanceFormRepository;

        // 工作流表单节点数据仓储
        private IRepository<WorkFlowInstanceNode, Guid> _workFlowInstanceNodeRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public WorkFlowInstanceService(IRepository<WorkFlowInstance, Guid> repository, IRepository<WorkFlowInstanceForm, Guid> workflowInstanceFormRepository, IRepository<WorkFlowInstanceNode, Guid> workFlowInstanceNodeRepository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _workflowInstanceFormRepository = workflowInstanceFormRepository;
            _workFlowInstanceNodeRepository = workFlowInstanceNodeRepository;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowInstanceAsync(WorkFlowInstance entity)
        {
            await _repository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task DeleteWorkFlowInstanceAsync(Guid id)
        {
            _repository.DeleteRange(e => e.Id == id);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public PagedData<WorkFlowInstance> GetWorkFlowInstances(int pageIndex, int pageSize, string searchText = "")
        {
            // 查询过滤
            var query = _repository.GetAllNoTracking();

            // 生成结果
            return _repository.GetByPage(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task UpdateWorkFlowInstanceAsync(WorkFlowInstance entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}

