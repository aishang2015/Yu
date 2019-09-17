
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WorkFlow;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Yu.Service.WorkFlow.WorkFlowTypes
{
    public class WorkFlowTypeService : IWorkFlowTypeService
    {

        // 仓储类
        private IRepository<WorkFlowType, Guid> _repository;
        private IRepository<WorkFlowDefine, Guid> _wfDefineRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        // 缓存
        private IMemoryCache _memoryCache;


        public WorkFlowTypeService(IRepository<WorkFlowType, Guid> repository,
            IRepository<WorkFlowDefine, Guid> wfDefineRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IMemoryCache memoryCache)
        {
            _repository = repository;
            _wfDefineRepository = wfDefineRepository;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
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
            _memoryCache.Remove(id);
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
            _memoryCache.Remove(entity.Id);
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

        /// <summary>
        /// 检查类型下是否定义了流程
        /// </summary>
        public bool HaveWorkFlowDefine(Guid id)
        {
            return 0 < _wfDefineRepository.GetByWhere(wfd => wfd.TypeId == id).Count();
        }

        /// <summary>
        /// 获取类型名称
        /// </summary>
        public string GetTypeNameById(Guid id)
        {
            return _memoryCache.GetOrCreate(id, entity =>
            {
                var typeName = from type in _repository.GetAllNoTracking()
                               where type.Id == id
                               select type.Name;
                return typeName.FirstOrDefault();
            });
        }
    }
}

