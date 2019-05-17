using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 数据仓储定义
    /// </summary>
    /// <typeparam name="TEntity">数据类型</typeparam>
    /// <typeparam name="TPrimaryKey">类型主键</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : BaseEntity<TPrimaryKey>

    {
        /// <summary>
        /// 通过主键获取对象
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>数据查询结果</returns>
        TEntity GetById(TPrimaryKey key);

        /// <summary>
        /// 全体数据查询
        /// </summary>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// 全体数据查询（不跟踪数据状态）
        /// </summary>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> GetAllNoTracking();

        /// <summary>
        /// 根据条件检索对象
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> GetByWhere(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 条件检索对象（不跟踪数据状态）
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> GetByWhereNoTracking(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 数据排序（正序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> OrderQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order);

        /// <summary>
        /// 数据排序（逆序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> OrderDescQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order);

        /// <summary>
        /// 取得分页数据
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>数据查询结果</returns>
        IQueryable<TEntity> GetByPage(IQueryable<TEntity> query, int pageIndex, int pageSize);

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>

        Task<TEntity> InsertAsync(TEntity entity);

        /// <summary>
        /// 插入一组数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="entity">删除的数据</param>
        void Delete(TEntity entity);

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">删除的数据</param>
        void DeleteRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="key">数据的主键</param>
        void Delete(TPrimaryKey key);

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="where">删除条件表达式</param>
        void DeleteRange(Expression<Func<TEntity, bool>> where);

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity">更新的对象</param>
        void Update(TEntity entity);

        /// <summary>
        /// 批量更新对象
        /// </summary>
        /// <param name="entities">批量更新的对象</param>
        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="properties">更新的属性表达式</param>
        void UpdatePartial(TEntity entity, Expression<Func<TEntity, object>>[] properties);
    }
}
