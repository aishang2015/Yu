
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
using System.Linq;

namespace Yu.Service.WorkFlow.WorkFlowDefines
{
    public class WorkFlowDefineService : IWorkFlowDefineService
    {

        // 仓储类
        private IRepository<WorkFlowDefine, Guid> _repository;

        // 仓储类
        private IRepository<WorkFlowFlowConnection, Guid> _connectionRepository;

        // 仓储类
        private IRepository<WorkFlowFlowNode, Guid> _nodeRepository;

        // 表单内容
        private IRepository<WorkFlowFormContent, Guid> _workflowFormContentRepository;

        // 表单元素
        private IRepository<WorkFlowFormElement, Guid> _workflowFormElementRepository;


        private IRepository<WorkFlowFlowNodeElement, Guid> _workFlowFlowNodeElementRepository;

        // 工作流类型服务
        private readonly IWorkFlowTypeService _workFlowTypeService;

        // 映射工具
        private readonly IMapper _mapper;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public WorkFlowDefineService(IRepository<WorkFlowDefine, Guid> repository,
            IRepository<WorkFlowFlowConnection, Guid> connectionRepository,
            IRepository<WorkFlowFlowNode, Guid> nodeRepository,
            IRepository<WorkFlowFormContent, Guid> workflowFormContentRepository,
            IRepository<WorkFlowFormElement, Guid> workflowFormElementRepository,
            IRepository<WorkFlowFlowNodeElement, Guid> workFlowFlowNodeElementRepository,
            IWorkFlowTypeService workFlowTypeService,
            IMapper mapper,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _connectionRepository = connectionRepository;
            _nodeRepository = nodeRepository;
            _workflowFormContentRepository = workflowFormContentRepository;
            _workflowFormElementRepository = workflowFormElementRepository;
            _workFlowFlowNodeElementRepository = workFlowFlowNodeElementRepository;
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
            _connectionRepository.DeleteRange(c => c.DefineId == id);
            _nodeRepository.DeleteRange(n => n.DefineId == id);
            _workflowFormContentRepository.DeleteRange(n => n.DefineId == id);
            _workflowFormElementRepository.DeleteRange(n => n.DefineId == id);
            _workFlowFlowNodeElementRepository.DeleteRange(n => n.DefineId == id);
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
        /// 取得数据
        /// </summary>
        public List<WorkflowDefineResult> GetAllWrokFlowDefines()
        {
            var result = _mapper.Map<List<WorkflowDefineResult>>(_repository
                .GetByWhereNoTracking(wfd => wfd.IsPublish).ToList());
            return result;
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task UpdateWorkFlowDefineAsync(WorkFlowDefine entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 设置工作流发布
        /// </summary>
        public async Task<bool> SetWorkFlowPublish(Guid id, bool isPublish)
        {
            // 验证是否设置表单内容
            var count = _workflowFormElementRepository
                .GetByWhereNoTracking(wffe => wffe.DefineId == id).Count();
            if (count == 0)
            {
                return false;
            }

            var nodes = _nodeRepository.GetByWhereNoTracking(wffn => wffn.DefineId == id).ToList();
            WorkFlowFlowNode node = nodes.Where(wffn => wffn.NodeType == 0).FirstOrDefault();
            while (true)
            {
                if (node == null)
                {
                    return false;
                }
                else
                {
                    if (node.NodeType == 99)
                    {
                        break;
                    }

                    // 到下一个节点的连接
                    var connection = _connectionRepository.GetByWhereNoTracking(c => c.DefineId == id && c.SourceId == node.NodeId)
                        .FirstOrDefault();

                    if (connection == null)
                    {
                        return false;
                    }

                    // 下一个节点
                    node = nodes.Where(wffn => wffn.NodeId == connection.TargetId).FirstOrDefault();
                }
            }

            // 验证是否有流程节点 
            count = _nodeRepository.GetByWhereNoTracking(wffn => wffn.DefineId == id).Count();
            if (count == 0)
            {
                return false;
            }

            var wfd = _repository.GetById(id);
            wfd.IsPublish = isPublish;
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}

