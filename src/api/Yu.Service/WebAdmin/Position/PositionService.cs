
using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Data.Entities.WebAdmin;
using Yu.Data.Infrasturctures.BaseIdentity;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Yu.Service.WebAdmin.Positions
{
    public class PositionService : IPositionService
    {

        // 仓储类
        private IRepository<Position, Guid> _repository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        // 缓存
        private readonly IMemoryCache _memoryCache;

        public PositionService(IRepository<Position, Guid> repository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IMemoryCache memoryCache)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        public async Task AddPositionAsync(Position entity)
        {
            await _repository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task DeletePositionAsync(Guid id)
        {
            _memoryCache.Remove(id);
            _repository.DeleteRange(e => e.Id == id);
            await _unitOfWork.CommitAsync();
        }


        /// <summary>
        /// 取得数据
        /// </summary>
        public List<Position> GetAllPositions()
        {
            return _repository.GetAllNoTracking().ToList();
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public PagedData<Position> GetPositions(int pageIndex, int pageSize, string searchText = "")
        {
            // 查询过滤
            var query = string.IsNullOrEmpty(searchText) ? _repository.GetAllNoTracking()
                : _repository.GetByWhereNoTracking(p => p.PositionName.Contains(searchText));

            // 生成结果
            return _repository.GetByPage(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task UpdatePositionAsync(Position entity)
        {
            _memoryCache.Remove(entity.Id);
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 检查名称重复
        /// </summary>
        public bool HaveRepeatName(Guid id, string positionName)
        {
            var count = _repository.GetByWhereNoTracking(p => p.Id != id && p.PositionName == positionName).Count();
            return count > 0;
        }

        /// <summary>
        /// 获取类型名称
        /// </summary>
        public string GetPositionNameById(Guid id)
        {
            return _memoryCache.GetOrCreate(id, entity =>
            {
                var positionNames = from type in _repository.GetAllNoTracking()
                               where type.Id == id
                               select type.PositionName;
                return positionNames.FirstOrDefault();
            });
        }

    }
}

