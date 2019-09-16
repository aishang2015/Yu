using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Group.InputModels;
using Yu.Model.WebAdmin.Group.OutputModels;
using G = Yu.Data.Entities.Right.Group;

namespace Yu.Service.WebAdmin.Group
{
    public class GroupService : IGroupService
    {
        private IRepository<G, Guid> _groupRepository;
        private IRepository<GroupTree, Guid> _groupTreeRepository;
        private IUnitOfWork<BaseIdentityDbContext> _unitOfWork;
        private readonly IMapper _mapper;

        public GroupService(IRepository<G, Guid> groupRepository,
            IRepository<GroupTree, Guid> groupTreeRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IMapper mapper)
        {
            _groupRepository = groupRepository;
            _groupTreeRepository = groupTreeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// 创建新组织
        /// </summary>
        public async Task CreateGroupAsync(GroupDetail groupDetail)
        {
            // 插入组织
            var group = await _groupRepository.InsertAsync(_mapper.Map<G>(groupDetail));

            // 上级组织树节点
            if (!string.IsNullOrEmpty(groupDetail.UpId))
            {
                var upId = Guid.Parse(groupDetail.UpId);
                var trees = _groupTreeRepository.GetByWhereNoTracking(gt => gt.Descendant == upId);
                foreach (var tree in trees)
                {
                    await _groupTreeRepository.InsertAsync(new GroupTree
                    {
                        Ancestor = tree.Ancestor,
                        Descendant = group.Id,
                        Length = tree.Length + 1
                    });
                }
            }

            // 自身组织树节点
            await _groupTreeRepository.InsertAsync(new GroupTree
            {
                Ancestor = group.Id,
                Descendant = group.Id,
                Length = 0
            });

            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        public async Task DeleteGroupAsync(Guid groupId)
        {
            // 检索自身和所有子元素的ID
            var groupIds = _groupTreeRepository.GetByWhereNoTracking(gt => gt.Ancestor == groupId).Select(gt => gt.Descendant);

            // 删除组织
            _groupRepository.DeleteRange(g => groupIds.Contains(g.Id));

            // 删除树
            _groupTreeRepository.DeleteRange(gt => groupIds.Contains(gt.Ancestor) || groupIds.Contains(gt.Descendant));

            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得所有组织
        /// </summary>
        public IEnumerable<GroupResult> GetAllGroups()
        {
            var result = new List<GroupResult>();
            var groups = _groupRepository.GetAllNoTracking();
            var groupTree = _groupTreeRepository.GetAllNoTracking().ToList();
            foreach (var group in groups)
            {
                var treeNode = groupTree.Where(gt => gt.Descendant == group.Id && gt.Length == 1).FirstOrDefault();
                result.Add(new GroupResult
                {
                    Id = group.Id.ToString(),
                    UpId = treeNode == null ? string.Empty : treeNode.Ancestor.ToString(),
                    GroupName = group.GroupName,
                    Remark = group.Remark
                });
            }
            return result;
        }

        /// <summary>
        /// 获取后代子树Id
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetDescendantGroup(Guid groupId)
        {
            var groupTrees = _groupTreeRepository.GetByWhereNoTracking(gt => gt.Ancestor == groupId);
            return groupTrees.Select(g => g.Descendant);
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        public async Task UpdateGroupAsync(GroupDetail groupDetail)
        {
            // 更新
            _groupRepository.Update(_mapper.Map<G>(groupDetail));
            await _unitOfWork.CommitAsync();
        }
    }
}
