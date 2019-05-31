using System.Collections.Generic;

namespace Yu.Data.Entities
{
    /// <summary>
    /// 存储分页数据
    /// </summary>
    /// <typeparam name="TBaseEntity"></typeparam>
    public class PagedData<TBaseEntity>
        where TBaseEntity : class
    {
        /// <summary>
        /// 全部数据条数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public List<TBaseEntity> Data { get; set; }
    }
}
