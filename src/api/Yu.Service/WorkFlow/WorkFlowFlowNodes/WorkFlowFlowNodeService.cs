
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Collections.Generic;
using System.Linq;

namespace Yu.Service.WorkFlow.WorkFlowFlowNodes
{
    public class WorkFlowFlowNodeService : IWorkFlowFlowNodeService
    {

        // 仓储类
        private IRepository<WorkFlowFlowNode, Guid> _repository;


        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;


        public WorkFlowFlowNodeService(IRepository<WorkFlowFlowNode, Guid> repository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowFlowNodesAsync(List<WorkFlowFlowNode> entities)
        {
            await _repository.InsertRangeAsync(entities);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteWorkFlowFlowNodeAsync(Guid id)
        {
            _repository.DeleteRange(e => e.DefineId == id);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public List<WorkFlowFlowNode> GetWorkFlowFlowNodes(Guid id)
        {
            // 查询过滤
            var result = _repository.GetByWhereNoTracking(wffn => wffn.DefineId == id);

            // 生成结果
            return result.ToList();
        }

        public async Task CommitAllAsync()
        {
            await _unitOfWork.CommitAsync();
        }
    }
}

