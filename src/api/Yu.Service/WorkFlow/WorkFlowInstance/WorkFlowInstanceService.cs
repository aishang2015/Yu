
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Yu.Core.Extensions;
using System.Collections.Generic;

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

        // 工作流节点定义
        private IRepository<WorkFlowFlowNode, Guid> _workFlowFlowNodeRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        IHttpContextAccessor _httpContextAccessor;

        public WorkFlowInstanceService(IRepository<WorkFlowInstance, Guid> repository,
            IRepository<WorkFlowInstanceForm, Guid> workflowInstanceFormRepository,
            IRepository<WorkFlowInstanceNode, Guid> workFlowInstanceNodeRepository,
            IRepository<WorkFlowFlowNode, Guid> workFlowFlowNodeRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _workflowInstanceFormRepository = workflowInstanceFormRepository;
            _workFlowInstanceNodeRepository = workFlowInstanceNodeRepository;
            _workFlowFlowNodeRepository = workFlowFlowNodeRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }



        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowInstanceAsync(WorkFlowInstance entity)
        {
            // 取得开始节点的数据
            var node = _workFlowFlowNodeRepository.GetByWhereNoTracking(wfn => wfn.DefineId == entity.DefineId && wfn.NodeType == "")
                .FirstOrDefault();
            entity.NodeId = node == null ? Guid.NewGuid() : node.Id;
            entity.OpenDate = DateTime.Now;
            await _repository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 更新或保存表单值
        /// </summary>
        public async Task AddOrUpdateWorkFlowInstanceForm(Guid instanceId, List<WorkFlowInstanceForm> forms)
        {
            forms.ForEach(wfif => wfif.InstanceId = instanceId);
            _workflowInstanceFormRepository.DeleteRange(wfif => wfif.InstanceId == instanceId);
            await _workflowInstanceFormRepository.InsertRangeAsync(forms);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得工作流实例表单值
        /// </summary>
        public List<WorkFlowInstanceForm> GetWorkFlowInstanceForm(Guid id)
        {
            return _workflowInstanceFormRepository.GetByWhereNoTracking(wfif => wfif.InstanceId == id).ToList();
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
            var query = _repository.GetByWhereNoTracking(wfi => wfi.UserName == _httpContextAccessor.HttpContext.User.GetUserName());

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

