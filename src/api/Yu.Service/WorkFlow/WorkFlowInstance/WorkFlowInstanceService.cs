
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
using Microsoft.EntityFrameworkCore;
using System.IO;
using Yu.Core.FileManage;

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

        // 工作流节点定义
        private IRepository<WorkFlowFlowNodeElement, Guid> _workflowFlowNodeElementRepository;

        // 工作流连接仓储
        private IRepository<WorkFlowFlowConnection, Guid> _workFlowFlowConnectionRepository;

        // 组织树仓储
        private IRepository<GroupTree, Guid> _groupTreeRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        // 文件管理
        private IFileStore _fileStore;

        IHttpContextAccessor _httpContextAccessor;

        public WorkFlowInstanceService(UserManager<BaseIdentityUser> userManager,
            IRepository<WorkFlowInstance, Guid> repository,
            IRepository<WorkFlowInstanceForm, Guid> workflowInstanceFormRepository,
            IRepository<WorkFlowInstanceNode, Guid> workFlowInstanceNodeRepository,
            IRepository<WorkFlowFlowNode, Guid> workFlowFlowNodeRepository,
            IRepository<WorkFlowFlowNodeElement, Guid> workflowFlowNodeElementRepository,
            IRepository<WorkFlowFlowConnection, Guid> workFlowFlowConnectionRepository,
            IRepository<GroupTree, Guid> groupTreeRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor,
            IFileStore fileStore)
        {
            _userManager = userManager;
            _repository = repository;
            _workflowInstanceFormRepository = workflowInstanceFormRepository;
            _workFlowInstanceNodeRepository = workFlowInstanceNodeRepository;
            _workFlowFlowNodeRepository = workFlowFlowNodeRepository;
            _workflowFlowNodeElementRepository = workflowFlowNodeElementRepository;
            _workFlowFlowConnectionRepository = workFlowFlowConnectionRepository;
            _groupTreeRepository = groupTreeRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _fileStore = fileStore;
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

            var n = nodes.Find(wffn => wffn.NodeType == 0);
            while (n != null)
            {
                var users = new List<BaseIdentityUser>();
                if (n.NodeType == 99)
                {
                    break;
                }
                else if (n.NodeType == 0)
                {
                    // 当前登录用户
                    var userName = _httpContextAccessor.HttpContext.User.GetUserName();
                    var currentUser = await _userManager.FindByNameAsync(userName);
                    users.Add(currentUser);
                }
                else
                {
                    // 取得经办人
                    if (n.HandleType == 1)
                    {
                        // 指定人员办理
                        users = (from user in _userManager.Users
                                 where n.HandlePeoples.Contains(user.Id.ToString())
                                 select user).ToList();
                    }
                    else if (n.HandleType == 2)
                    {
                        // 当前登录用户
                        var userName = _httpContextAccessor.HttpContext.User.GetUserName();
                        var currentUser = await _userManager.FindByNameAsync(userName);

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
                    }

                }

                users.ForEach(async user =>
                {
                    await _workFlowInstanceNodeRepository.InsertAsync(new WorkFlowInstanceNode
                    {
                        InstanceId = entity.Id,
                        NodeId = n.Id,
                        HandlePeople = user.Id.ToString(),
                        HandlePeopleName = user.FullName,
                        Explain = string.Empty,
                        HandleStatus = n.NodeType == 0 ? 1 : 0,
                        CreateDateTime = DateTime.Now
                    });
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
                            HandlePeople = wfin.HandlePeople,
                            HandlePeopleName = wfin.HandlePeopleName,
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

        /// <summary>
        /// 处理工作流状态
        /// </summary>
        public async Task HandleWorkFlowInstance(Guid instanceId, int handleStatus, string explain)
        {
            // 当前登录用户
            var userName = _httpContextAccessor.HttpContext.User.GetUserName();
            var currentUser = _userManager.Users.AsNoTracking().FirstOrDefault(u => u.UserName == userName);

            // 获取实例对象
            var instance = _repository.GetByWhereNoTracking(i => i.Id == instanceId).FirstOrDefault();

            // 取得待处理节点数据
            var result = (from instanceNode in _workFlowInstanceNodeRepository.GetAllNoTracking()
                          where instanceNode.InstanceId == instanceId && instanceNode.HandleStatus == 1
                                 && instanceNode.HandlePeople == currentUser.Id.ToString()
                          select instanceNode).FirstOrDefault();
            if (result != null)
            {
                result.HandleStatus = handleStatus;
                result.Explain = explain;
                result.HandleDateTime = DateTime.Now;
                switch (handleStatus)
                {
                    // 拒绝
                    case 2:

                        // 重置所有处理状态回到开始节点
                        // 所有实例节点
                        var instanceNodes = _workFlowInstanceNodeRepository.GetByWhere(n => n.InstanceId == instanceId && n.Id != result.Id)
                            .ToList();

                        // 开始节点数据
                        var startNode = (from node in _workFlowFlowNodeRepository.GetAllNoTracking()
                                         where node.DefineId == instance.DefineId && node.NodeType == 0
                                         select node).FirstOrDefault();


                        // 实例设置
                        instance.NodeId = startNode.Id;
                        instance.State = 3;

                        foreach (var n in instanceNodes)
                        {
                            if (n.NodeId == startNode.Id)
                            {
                                n.HandleStatus = 1;
                            }
                            else
                            {
                                n.HandleStatus = 0;
                            }
                        }

                        instanceNodes.Add(result);
                        _workFlowInstanceNodeRepository.UpdateRange(instanceNodes);
                        break;

                    // 同意
                    case 3:

                        // 查找当前节点会签情况
                        var count = (from instanceNode in _workFlowInstanceNodeRepository.GetAllNoTracking()
                                     where instanceNode.InstanceId == instanceId &&
                                             instanceNode.NodeId == result.NodeId &&
                                             instanceNode.HandleStatus == 1
                                     select instanceNode).Count();

                        // 只剩当前处理则直接进入下一个节点
                        if (count == 1)
                        {
                            // 当前节点
                            var currentNode = _workFlowFlowNodeRepository.GetByWhereNoTracking(n => n.Id == result.NodeId)
                                .FirstOrDefault();

                            // 下一个节点
                            var nextNode = (from wffc in _workFlowFlowConnectionRepository.GetAllNoTracking()
                                            join wffn in _workFlowFlowNodeRepository.GetAllNoTracking() on wffc.TargetId equals wffn.NodeId
                                            where wffc.DefineId == instance.DefineId && wffc.SourceId == currentNode.NodeId
                                            select wffn).FirstOrDefault();

                            // 不存在
                            if (nextNode == null)
                            {
                                break;
                            }

                            instance.NodeId = nextNode.Id;

                            // 如果下个节点是结束节点
                            if (nextNode.NodeType == 99)
                            {
                                instance.State = 4;
                                _workFlowInstanceNodeRepository.Update(result);
                            }
                            else
                            {
                                instance.State = 2;

                                // 下一步要处理的节点
                                var nextInstanceNodes = (from instanceNode in _workFlowInstanceNodeRepository.GetAllNoTracking()
                                                         where instanceNode.InstanceId == instanceId
                                                                 && instanceNode.NodeId == nextNode.Id
                                                         select instanceNode).ToList();

                                // 更新成待处理
                                foreach (var instanceNode in nextInstanceNodes)
                                {
                                    instanceNode.HandleStatus = 1;
                                    instanceNode.Explain = string.Empty;
                                }

                                nextInstanceNodes.Add(result);
                                _workFlowInstanceNodeRepository.UpdateRange(nextInstanceNodes);
                            }
                        }
                        else if (count > 1)
                        {
                            // 会签的情况
                            // 只更新当前节点状态
                            _workFlowInstanceNodeRepository.Update(result);
                        }

                        break;
                }

                _repository.Update(instance);
                await _unitOfWork.CommitAsync();
            }
        }

        /// <summary>
        /// 取得待办数据
        /// </summary>
        public PagedData<WorkFlowInstance> GetHandleWorkFlowInstances(int pageIndex, int pageSize, string searchText)
        {
            // 当前登录用户
            var userName = _httpContextAccessor.HttpContext.User.GetUserName();
            var currentUser = _userManager.Users.AsNoTracking().FirstOrDefault(u => u.UserName == userName);

            // 取得结果
            var result = from instance in _repository.GetAllNoTracking()
                         join node in _workFlowInstanceNodeRepository.GetAllNoTracking() on instance.Id equals node.InstanceId
                         where node.HandleStatus == 1 && node.HandlePeople == currentUser.Id.ToString() && instance.State == 2
                         orderby instance.OpenDate
                         select instance;

            return new PagedData<WorkFlowInstance>
            {
                Data = result.Skip((pageIndex - 1) * pageSize).ToList(),
                Total = result.Count()
            };
        }

        /// <summary>
        /// 取得工作流节点元素设置
        /// </summary>
        /// <returns></returns>
        public List<WorkFlowFlowNodeElement> GetWorkFlowNodeElements(Guid instanceId)
        {
            var node = (from instance in _repository.GetAllNoTracking()
                        join n in _workFlowFlowNodeRepository.GetAllNoTracking() on instance.NodeId equals n.Id
                        where instance.Id == instanceId
                        select n).FirstOrDefault();
            if (node != null)
            {
                var result = from ne in _workflowFlowNodeElementRepository.GetAllNoTracking()
                             where ne.DefineId == node.DefineId && ne.FlowNodeId == node.NodeId
                             select ne;
                return result.ToList();
            }
            return null;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public async Task<string> AddWorkFlowInstanceFormFile(IFormFile file)
        {
            var endfix = file.FileName.Split('.').Last();
            var newName = Path.Combine(DateTime.Now.ToString("MMddHHmmssfff") + '.' + endfix);
            await _fileStore.CreateFile(newName, @"C:\temp\files\wf", file.OpenReadStream());
            return newName;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        public void RemoveWorkFlowInstanceFormFile(string fileName){
            _fileStore.DeleteFile(fileName, @"C:\temp\files\wf");
        }



    }
}

