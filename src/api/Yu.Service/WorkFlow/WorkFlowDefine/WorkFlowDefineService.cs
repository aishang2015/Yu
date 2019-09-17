
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using AutoMapper;
using Yu.Service.WorkFlow.WorkFlowTypes;
using System.Collections.Generic;
using Yu.Model.WorkFlow.WorkFlowDefine.OutputModels;

namespace Yu.Service.WorkFlow.WorkFlowDefines
{
    public class WorkFlowDefineService : IWorkFlowDefineService
    {

        // 仓储类
        private IRepository<WorkFlowDefine, Guid> _repository;

        // 工作流类型服务
        private readonly IWorkFlowTypeService _workFlowTypeService;

        // 映射工具
        private readonly IMapper _mapper;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public WorkFlowDefineService(IRepository<WorkFlowDefine, Guid> repository,
            IWorkFlowTypeService workFlowTypeService,
            IMapper mapper,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _workFlowTypeService = workFlowTypeService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// 取得数据
        /// </summary>
        public WorkFlowDefine GetWorkFlowById(Guid id)
        {
            return _repository.GetById(id);
        }


        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowDefineAsync(WorkFlowDefine entity)
        {
            await _repository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task DeleteWorkFlowDefineAsync(Guid id)
        {
            _repository.DeleteRange(e => e.Id == id);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public PagedData<WorkflowDefineResult> GetWorkFlowDefines(int pageIndex, int pageSize, string typeId = "")
        {
            // 查询过滤
            var query = string.IsNullOrEmpty(typeId) ? _repository.GetAllNoTracking()
                : _repository.GetByWhereNoTracking(wfd => wfd.TypeId == Guid.Parse(typeId));

            // 生成结果
            var wfDefines = _repository.GetByPage(query, pageIndex, pageSize);
            var result = _mapper.Map<List<WorkflowDefineResult>>(wfDefines.Data);

            result.ForEach(wfdr => wfdr.TypeName = _workFlowTypeService.GetTypeNameById(wfdr.TypeId));

            return new PagedData<WorkflowDefineResult>
            {
                Data = result,
                Total = wfDefines.Total
            };
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task UpdateWorkFlowDefineAsync(WorkFlowDefine entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}

