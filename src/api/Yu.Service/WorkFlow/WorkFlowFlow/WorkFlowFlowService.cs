using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;

namespace Yu.Service.WorkFlow.WorkFlowFlow
{
    public class WorkFlowFlowService : IWorkFlowFlowService
    {
        // 仓储类
        private IRepository<WorkFlowFlowConnection, Guid> _connectionRepository;

        // 仓储类
        private IRepository<WorkFlowFlowNode, Guid> _nodeRepository;

        // 仓储类
        private IRepository<WorkFlowFlowNodeElement, Guid> _nodeElementRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public WorkFlowFlowService(IRepository<WorkFlowFlowConnection, Guid> connectionRepository,
            IRepository<WorkFlowFlowNode, Guid> nodeRepository,
            IRepository<WorkFlowFlowNodeElement, Guid> nodeElementRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _connectionRepository = connectionRepository;
            _nodeRepository = nodeRepository;
            _nodeElementRepository = nodeElementRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowFlowConnectionsAsync(List<WorkFlowFlowConnection> entities)
        {
            await _connectionRepository.InsertRangeAsync(entities);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteWorkFlowFlowConnectionAsync(Guid id)
        {
            _connectionRepository.DeleteRange(e => e.DefineId == id);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public List<WorkFlowFlowConnection> GetWorkFlowFlowConnections(Guid id)
        {
            // 查询过滤
            var query = _connectionRepository.GetByWhereNoTracking(wffc => wffc.DefineId == id);

            // 生成结果
            return query.ToList();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowFlowNodesAsync(List<WorkFlowFlowNode> entities)
        {
            await _nodeRepository.InsertRangeAsync(entities);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteWorkFlowFlowNodeAsync(Guid id)
        {
            _nodeRepository.DeleteRange(e => e.DefineId == id);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public List<WorkFlowFlowNode> GetWorkFlowFlowNodes(Guid id)
        {
            // 查询过滤
            var result = _nodeRepository.GetByWhereNoTracking(wffn => wffn.DefineId == id);

            // 生成结果
            return result.ToList();
        }


        /// <summary>
        /// 保存流程
        /// </summary>
        public async Task AddOrUpdateFlow(Guid defineId, List<WorkFlowFlowNode> nodes, List<WorkFlowFlowConnection> connections, List<WorkFlowFlowNodeElement> nodeElements)
        {
            DeleteWorkFlowFlowNodeAsync(defineId);
            await AddWorkFlowFlowNodesAsync(nodes);
            DeleteWorkFlowFlowConnectionAsync(defineId);
            await AddWorkFlowFlowConnectionsAsync(connections);
            DeleteWorkFlowFlowNodeElementAsync(defineId);
            await AddWorkFlowFlowNodeElementsAsync(nodeElements);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        public List<WorkFlowFlowNodeElement> GetWorkFlowFlowNodeElements(Guid id)
        {
            // 查询过滤
            var result = _nodeElementRepository.GetByWhereNoTracking(wffne => wffne.DefineId == id);

            // 生成结果
            return result.ToList();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public void DeleteWorkFlowFlowNodeElementAsync(Guid id)
        {
            _nodeElementRepository.DeleteRange(e => e.DefineId == id);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowFlowNodeElementsAsync(List<WorkFlowFlowNodeElement> nodeElements)
        {
            await _nodeElementRepository.InsertRangeAsync(nodeElements);
        }


    }
}
