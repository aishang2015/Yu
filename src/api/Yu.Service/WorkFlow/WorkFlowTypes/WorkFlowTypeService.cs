
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using System.Collections.Generic;

namespace Yu.Service.WorkFlow.WorkFlowTypes
{
    public class WorkFlowTypeService : IWorkFlowTypeService
    {

        // 仓储类
        private IRepository<WorkFlowType, Guid> _repository;


        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;


        public WorkFlowTypeService(IRepository<WorkFlowType, Guid> repository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddWorkFlowTypeAsync(WorkFlowType entity)
        {
            await _repository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task DeleteWorkFlowTypeAsync(Guid id)
        {
            _repository.DeleteRange(e => e.Id == id);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public List<WorkFlowType> GetWorkFlowTypes()
        {
            // 查询过滤
            var query = _repository.OrderQuery(_repository.GetAllNoTracking(), w => w.Order);

            // 生成结果
            return query.ToList();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task UpdateWorkFlowTypeAsync(WorkFlowType entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 检查名称是否重复
        /// </summary>
        public bool HaveRepeatName(Guid id, string name)
        {
            var data = from wfType in _repository.GetAllNoTracking()
                       where wfType.Id != id && wfType.Name == name
                       select wfType;
            return data.Count() > 0;
        }
    }
}

