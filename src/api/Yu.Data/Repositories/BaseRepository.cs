using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Data.Entities;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 数据仓储基本实现
    /// </summary>
    /// <typeparam name="TDbContextKeyType">认证数据库上下文主键类型</typeparam>
    /// <typeparam name="TEntity">数据类型</typeparam>
    /// <typeparam name="TPrimaryKey">类型主键</typeparam>
    public class BaseRepository<TEntity, TPrimaryKey, TDbContext> : IRepository<TEntity, TPrimaryKey>
        where TEntity : BaseEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        private readonly DbContext _context;

        private readonly DbSet<TEntity> _dataSet;

        private readonly Expression<Func<TEntity, bool>> _condition;

        public BaseRepository(IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _context = httpContextAccessor.HttpContext.RequestServices.GetService<TDbContext>();

            _dataSet = _context.Set<TEntity>();

            var userName = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserName)?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                // 表达式合集
                var expressionList = new List<LambdaExpression>();

                // 取得用户的数据规则
                var result = memoryCache.TryGetValue(CommonConstants.RoleMemoryCacheKey + userName, out List<string> permissions);

                // 所有规则组
                foreach (var group in permissions)
                {
                    // 找到符合当前实体的规则
                    var items = group.Split('|');
                    if (items[0] == typeof(TDbContext).Name && items[1] == typeof(TEntity).Name)
                    {
                        // 表达式反序列化
                        var serializer = new ExpressionSerializer(new JsonSerializer());
                        serializer.AddKnownType(typeof(Core.Expressions.ExpressionType));
                        var lambda = (LambdaExpression)serializer.DeserializeText(items[2]);
                        expressionList.Add(lambda);
                    }
                }

                // 连接lambda表达式生成统一条件
                if (expressionList.Count > 0)
                {
                    _condition = (Expression<Func<TEntity, bool>>)(new ExpressionUtil<TEntity>()
                        .JoinLambdaExpression(expressionList, ExpressionCombineType.And));
                }
            }

        }


        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="entity">删除的数据</param>
        public void Delete(TEntity entity)
        {
            _dataSet.Remove(entity);
        }

        /// <summary>
        /// 根据Id删除
        /// </summary>
        /// <param name="key">数据的主键</param>
        public void Delete(TPrimaryKey key)
        {
            Delete(GetById(key));
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">删除的数据</param>
        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            _dataSet.RemoveRange(entities);
        }

        /// <summary>
        /// 根据条件删除
        /// </summary>
        /// <param name="where">删除条件表达式</param>
        public void DeleteRange(Expression<Func<TEntity, bool>> where)
        {
            DeleteRange(GetByWhere(where));
        }

        /// <summary>
        /// 全体数据查询
        /// </summary>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetAll()
        {
            return _condition == null ? _dataSet : _dataSet.Where(_condition);
        }

        /// <summary>
        /// 全体数据查询（不跟踪数据状态）
        /// </summary>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetAllNoTracking()
        {
            return _condition == null ? _dataSet.AsNoTracking() : _dataSet.AsNoTracking().Where(_condition);
        }

        /// <summary>
        /// 通过主键获取对象
        /// </summary>
        /// <param name="key">主键值</param>
        /// <returns>数据查询结果</returns>
        public TEntity GetById(TPrimaryKey key)
        {
            return _condition == null ? _dataSet.Find(key) : GetAllNoTracking().FirstOrDefault(e => e.Id.Equals(key));
        }

        /// <summary>
        /// 取得分页数据
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>数据查询结果</returns>
        public PagedData<TEntity> GetByPage(IQueryable<TEntity> query, int pageIndex, int pageSize)
        {
            var skip = pageSize * (pageIndex - 1);
            return new PagedData<TEntity>
            {
                Total = query.Count(), // 满足条件总条数
                Data = query.Skip(skip).Take(pageSize).ToList()
            };
        }

        /// <summary>
        /// 根据条件检索对象
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetByWhere(Expression<Func<TEntity, bool>> where)
        {
            return _condition == null ? _dataSet.Where(where) : _dataSet.Where(_condition).Where(where);
        }

        /// <summary>
        /// 条件检索对象（不跟踪数据状态）
        /// </summary>
        /// <param name="where">检索条件</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> GetByWhereNoTracking(Expression<Func<TEntity, bool>> where)
        {
            return GetAllNoTracking().Where(where);
        }

        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await _dataSet.AddAsync(entity);
            return result.Entity;
        }

        /// <summary>
        /// 插入一组数据
        /// </summary>
        /// <param name="entity">对象数据</param>
        /// <returns>实体对象</returns>
        public async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dataSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// 数据排序（逆序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> OrderDescQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order)
        {
            return query.OrderByDescending(order);
        }

        /// <summary>
        /// 数据排序（正序）
        /// </summary>
        /// <param name="query">原始队列</param>
        /// <param name="order">排序表达式</param>
        /// <returns>数据查询结果</returns>
        public IQueryable<TEntity> OrderQuery(IQueryable<TEntity> query, Expression<Func<TEntity, object>> order)
        {
            return query.OrderBy(order);
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <param name="entity">更新的对象</param>
        public void Update(TEntity entity)
        {
            // 关联实体,然后设置状态
            _dataSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 更新部分属性
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="properties">更新的属性表达式</param>
        public void UpdatePartial(TEntity entity, Expression<Func<TEntity, object>>[] properties)
        {
            // 关联实体,然后设置状态
            _dataSet.Attach(entity);
            foreach (var property in properties)
            {
                _context.Entry(entity).Property(property).IsModified = true;
            }
        }

        /// <summary>
        /// 批量更新对象
        /// </summary>
        /// <param name="entities">批量更新的对象</param>
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dataSet.UpdateRange(entities);
        }
    }
}