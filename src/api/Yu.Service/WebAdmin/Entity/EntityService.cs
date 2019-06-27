using AutoMapper;
using System;
using System.Collections.Generic;
using Yu.Data.Repositories;
using Yu.Model.WebAdmin.Entity.OutputModels;
using EntityData = Yu.Data.Entities.Right.Entity;

namespace Yu.Service.WebAdmin.Entity
{
    public class EntityService : IEntityService
    {
        private readonly IRepository<EntityData, Guid> _entityRepository;

        public EntityService(IRepository<EntityData, Guid> entityRepository)
        {
            _entityRepository = entityRepository;
        }

        /// <summary>
        /// 取得所有的实体数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EntityOutline> GetAllEntityOutline()
        {
            var entities = _entityRepository.GetAllNoTracking();
            return Mapper.Map<List<EntityOutline>>(entities);
        }
    }
}
