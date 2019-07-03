using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Model.WebAdmin.Entity.OutputModels;
using EntityE = Yu.Data.Entities.Right.Entity;

namespace Yu.Service.WebAdmin.Entity
{
    public interface IEntityService
    {
        IEnumerable<EntityOutline> GetAllEntityOutline();

        /// <summary>
        /// 取得实体
        /// </summary>
        PagedData<EntityE> GetEntities(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 更新实体
        /// </summary>
        Task UpdateEntity(EntityE entity);

        /// <summary>
        /// 插入实体
        /// </summary>
        Task InsertEntity(EntityE entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        Task DeleteEntity(Guid entityId);
    }
}
