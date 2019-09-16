using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Entity.OutputModels;
using EntityData = Yu.Data.Entities.Right.Entity;

namespace Yu.Service.WebAdmin.Entity
{
    public class EntityService : IEntityService
    {
        private readonly IRepository<EntityData, Guid> _entityRepository;

        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        private readonly IMapper _mapper;

        public EntityService(IRepository<EntityData, Guid> entityRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IMapper mapper)
        {
            _entityRepository = entityRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        /// <summary>
        /// 删除实体
        /// </summary>
        public async Task DeleteEntityAsync(Guid entityId)
        {
            _entityRepository.DeleteRange(e => e.Id == entityId);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 取得所有的实体数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntityOutline> GetAllEntityOutline()
        {
            var entities = _entityRepository.GetAllNoTracking();
            return _mapper.Map<List<EntityOutline>>(entities);
        }

        /// <summary>
        /// 取得实体
        /// </summary>
        public PagedData<EntityData> GetEntities(int pageIndex, int pageSize, string searchText)
        {
            var expressionGroup = new ExpressionGroup<EntityData>()
            {
                ExpressionCombineType = ExpressionCombineType.Or,
                TupleList = new List<(string, object, ExpressionType)>
                  {
                      ("DbContext",searchText,ExpressionType.Equal),
                      ("Table",searchText,ExpressionType.Equal),
                      ("Field",searchText,ExpressionType.Equal),
                  }
            };

            var filter = expressionGroup.GetLambda();

            var query = _entityRepository.GetByWhereNoTracking(filter);
            var result = _entityRepository.GetByPage(query, pageIndex, pageSize);
            return result;
        }

        /// <summary>
        /// 插入实体
        /// </summary>
        public async Task InsertEntityAsync(EntityData entity)
        {
            await _entityRepository.InsertAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        public async Task UpdateEntityAsync(EntityData entity)
        {
            _entityRepository.Update(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
