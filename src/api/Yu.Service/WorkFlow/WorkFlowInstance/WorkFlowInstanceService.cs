
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Yu.Core.Extensions;
using System.Collections.Generic;
using Yu.Core.Utils;
using Microsoft.AspNetCore.Identity;
using Yu.Data.Entities.Right;
using Yu.Model.WorkFlow.WorkFlowInstance.OutputModels;

namespace Yu.Service.WorkFlow.WorkFlowInstances
{
    public class WorkFlowInstanceService : IWorkFlowInstanceService
    {
        // 用户
        private UserManager<BaseIdentityUser> _userManager;

        // 仓储类
        private IRepository<WorkFlowInstance, Guid> _repository;

        // 工作流表单值仓储
        private IRepository<WorkFlowInstanceForm, Guid> _workflowInstanceFormRepository;

        // 工作流表单节点数据仓储
        private IRepository<WorkFlowInstanceNode, Guid> _workFlowInstanceNodeRepository;

        // 工作流节点定义
        private IRepository<WorkFlowFlowNode, Guid> _workFlowFlowNodeRepository;

        // 工作流连接仓储
        private IRepository<WorkFlowFlowConnection, Guid> _workFlowFlowConnectionRepository;

        // 组织树仓储
        private IRepository<GroupTree, Guid> _groupTreeRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        IHttpContextAccessor _httpContextAccessor;

        public WorkFlowInstanceService(UserManager<BaseIdentityUser> userManager,
            IRepository<WorkFlowInstance, Guid> repository,
            IRepository<WorkFlowInstanceForm, Guid> workflowInstanceFormRepository,
            IRepository<WorkFlowInstanceNode, Guid> workFlowInstanceNodeRepository,
            IRepository<WorkFlowFlowNode, Guid> workFlowFlowNodeRepository,
            IRepository<WorkFlowFlowConnection, Guid> workFlowFlowConnectionRepository,
            IRepository<GroupTree, Guid> groupTreeRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _repository = repository;
            _workflowInstanceFormRepository = workflowInstanceFormRepository;
            _workFlowInstanceNodeRepository = workFlowInstanceNodeRepository;
            _workFlowFlowNodeRepository = workFlowFlowNodeRepository;
            _workFlowFlowConnectionRepository = workFlowFlowConnectionRepository;
            _groupTreeRepository = groupTreeRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowInstanceAsync(WorkFlowInstance entity)
        {
            // 取得开始节点的数据
            var node = _workFlowFlowNodeRepository.GetByWhereNoTracking(wfn => wfn.DefineId == entity.DefineId)
                .FirstOrDefault();
            entity.Id = GuidUtil.NewSquentialGuid();
            entity.NodeId = node == null ? Guid.NewGuid() : node.Id;
            entity.State = 1;
            entity.OpenDate = DateTime.Now;

            // 设置实例
            var nodes = _workFlowFlowNodeRepository.GetByWhereNoTracking(wffn => wffn.DefineId == node.DefineId).ToList();

            var n = nodes.Find(wffn => wffn.NodeType == "startNode");
            while (n != null)
            {
                var handlePepleIds = string.Empty;
                var handlePepleNames = string.Empty;
                if (n.NodeType == "endNode")
                {
                    break;
                }
                else if (n.NodeType == "startNode")
                {
                    // 当前登录用户
                    var userName = _httpContextAccessor.HttpContext.User.GetUserName();
                    var currentUser = await _userManager.FindByNameAsync(userName);
                    handlePepleIds = currentUser.Id.ToString();
                    handlePepleNames = currentUser.FullName;
                }
                else
                {
                    // 取得经办人
                    if (n.HandleType == 1)
                    {
                        // 指定人员办理
                        handlePepleIds = n.HandlePeoples;
                        handlePepleNames = string.Join(",", from user in _userManager.Users
                                                            where handlePepleIds.Contains(user.Id.ToString())
                                                            select user.FullName);
                    }
                    else if (n.HandleType == 2)
                    {
                        // 当前登录用户
                        var userName = _httpContextAccessor.HttpContext.User.GetUserName();
                        var currentUser = await _userManager.FindByNameAsync(userName);
                        var users = new List<BaseIdentityUser>();

                        // 指定岗位人员办理
                        switch (n.PositionGroup)
                        {
                            // 不指定部门
                            case 1:
                                // 直接根据岗位信息查找人员
                                users.AddRange(_userManager.Users.Where(u => u.PositionId == n.PositionId));
                                break;
                            // 发起人部门的指定岗位
                            case 2:
                                users.AddRange(from user in _userManager.Users
                                               where user.PositionId == n.PositionId &&
                                                   user.UserGroupId == currentUser.UserGroupId
                                               select user);
                                break;
                            // 发起人部门上一级部门的指定岗位
                            case 3:
                                var upperGroupId = _groupTreeRepository
                                    .GetByWhereNoTracking(gt => gt.Descendant.ToString() == currentUser.UserGroupId)
                                    .FirstOrDefault()?.Ancestor;
                                if (upperGroupId != null)
                                {
                                    users.AddRange(from user in _userManager.Users
                                                   where user.PositionId == n.PositionId &&
                                                       user.UserGroupId == upperGroupId.ToString()
                                                   select user);
                                }
                                break;
                            default:
                                break;
                        }
                        handlePepleIds = string.Join(",", users.Select(u => u.Id));
                        handlePepleNames = string.Join(",", users.Select(u => u.FullName));
                    }

                }


                await _workFlowInstanceNodeRepository.InsertAsync(new WorkFlowInstanceNode
                {
                    InstanceId = entity.Id,
                    NodeId = n.Id,
                    HandlePeoples = handlePepleIds,
                    HandlePeopleNames = handlePepleNames,
                    Explain = string.IsNullOrEmpty(handlePepleIds) ? "没有找到匹配的经办人员,略过改步骤." : string.Empty,
                    HandleStatus = 0,
                    CreateDateTime =DateTime.Now
                });

                var connection = _workFlowFlowConnectionRepository
                    .GetByWhereNoTracking(wffc => wffc.DefineId == entity.DefineId 
                    && wffc.SourceId == n.NodeId.ToString()).FirstOrDefault();
                n = connection == null ? null : nodes.FirstOrDefault(nd => nd.NodeId.ToString() == connection.TargetId);
            }

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
        public List<WorkFlowInstanceForm> GetWorkFlowInstanceForm(Guid instanceId)
        {
            return _workflowInstanceFormRepository.GetByWhereNoTracking(wfif => wfif.InstanceId == instanceId).ToList();
        }

        /// <summary>
        /// 取得工作流实例节点处理数据
        /// </summary>
        public List<WorkFlowInstanceNodeResult> GetWorkFlowInstanceNode(Guid instanceId)
        {
            var wfins = from wfin in _workFlowInstanceNodeRepository.GetAllNoTracking()
                        join wffn in _workFlowFlowNodeRepository.GetAllNoTracking() on wfin.NodeId equals wffn.Id
                        where wfin.InstanceId == instanceId
                        orderby wfin.CreateDateTime
                        select new WorkFlowInstanceNodeResult
                        {
                            NodeName = wffn.Name,
                            HandlePeoples = wfin.HandlePeoples,
                            HandlePeopleNames = wfin.HandlePeopleNames,
                            HandleStatus = wfin.HandleStatus,
                            Explain = wfin.Explain,
                            HandleDateTime = wfin.HandleDateTime
                        };

            return wfins.ToList();

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task DeleteWorkFlowInstanceAsync(Guid id)
        {
            _repository.DeleteRange(e => e.Id == id);
            _workflowInstanceFormRepository.DeleteRange(e => e.InstanceId == id);
            _workFlowInstanceNodeRepository.DeleteRange(e => e.InstanceId == id);
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
            var query = from instance in _repository.GetAllNoTracking()
                        where instance.UserName == _httpContextAccessor.HttpContext.User.GetUserName() && !instance.IsDelete
                        orderby instance.OpenDate descending
                        select instance;

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

        /// <summary>
        /// 取得被删除的工作流实例
        /// </summary>
        public PagedData<WorkFlowInstance> GetDeletedWorkFlowInstanceForm(int pageIndex, int pageSize, string searchText)
        {

            // 查询过滤
            var query = _repository.GetByWhereNoTracking(wfi => wfi.UserName == _httpContextAccessor.HttpContext.User.GetUserName() && wfi.IsDelete);

            // 生成结果
            return _repository.GetByPage(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 设置工作流实例删除位
        /// </summary>
        public async Task<bool> SetWorkFlowInstanceDelete(Guid id, bool isDelete)
        {
            var instance = _repository.GetByWhere(wfi => wfi.Id == id).FirstOrDefault();
            if (instance.State == 2)
            {
                return false;
            }

            instance.IsDelete = isDelete;
            _repository.Update(instance);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}

