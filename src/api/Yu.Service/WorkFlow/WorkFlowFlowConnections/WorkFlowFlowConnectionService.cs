
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using System.Collections.Generic;

namespace Yu.Service.WorkFlow.WorkFlowFlowConnections
{
    public class WorkFlowFlowConnectionService : IWorkFlowFlowConnectionService
    {

        // 仓储类
        private IRepository<WorkFlowFlowConnection, Guid> _repository;


        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;


        public WorkFlowFlowConnectionService(IRepository<WorkFlowFlowConnection, Guid> repository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowFlowConnectionsAsync(List<WorkFlowFlowConnection> entities)
        {
            await _repository.InsertRangeAsync(entities);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteWorkFlowFlowConnectionAsync(Guid id)
        {
            _repository.DeleteRange(e => e.DefineId == id);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public List<WorkFlowFlowConnection> GetWorkFlowFlowConnections(Guid id)
        {
            // 查询过滤
            var query = _repository.GetByWhere(wffc => wffc.DefineId == id);

            // 生成结果
            return query.ToList();
        }
    }
}

