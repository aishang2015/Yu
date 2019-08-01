using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Yu.Core.Extensions;
using Yu.Data.Entities;

namespace Yu.Data.Infrasturctures
{
    public static class EntityTypeFinder
    {
        /// <summary>
        /// 查找dbcontext所属的entity
        /// </summary>
        /// <param name="dbcontextType">dbcontext类型</param>
        /// <returns>entity类型合集</returns>
        public static List<Type> FindEntityTypes(Type dbcontextType = null)
        {
            // 寻找entity
            var typeList = typeof(BaseEntity<>).GetAllChildType();

            // 不指定类型时返回所有的baseentity的实现
            if (dbcontextType == null)
            {
                return typeList;
            }
            else
            {
                var entityTypeList = new List<Type>();
                foreach (var type in typeList)
                {
                    // 判断是否有belongto特性
                    var attribute = type.GetCustomAttributes(typeof(BelongToAttribute), false).FirstOrDefault();
                    if (attribute != null)
                    {
                        // 判断belongto特性是否和当前dbcontext相同
                        if (((BelongToAttribute)attribute).DbContextType == dbcontextType)
                        {
                            entityTypeList.Add(type);
                        }
                    }
                }
                return entityTypeList;
            }
        }

        /// <summary>
        /// 根据数据库名称和表名查找类型
        /// </summary>
        /// <param name="dbContextName">Dbcontext名称</param>
        /// <param name="entityName">实体名称</param>
        /// <returns></returns>
        public static Type FindEntityType(string dbContextName, string entityName)
        {
            // 寻找entity
            var typeList = typeof(BaseEntity<>).GetAllChildType();

            // 类型列表
            foreach (var type in typeList)
            {
                // 判断实体名称是否一致
                if (type.Name == entityName)
                {
                    // 判断dbcontext类型是否一致
                    var attribute = type.GetCustomAttributes(typeof(BelongToAttribute), false).FirstOrDefault();
                    if (attribute != null)
                    {
                        // 判断belongto特性是否和当前dbcontext相同
                        if (((BelongToAttribute)attribute).DbContextType.Name == dbContextName)
                        {
                            return type;
                        }
                    }
                }
            }

            // 没有找到
            return null;
        }

        /// <summary>
        /// 查找所有拥有[DataRuleManage]特性的实体
        /// </summary>
        /// <returns></returns>
        public static List<Type> FindDataRuleManageEntityTypes()
        {
            var typeList = FindEntityTypes();
            var result = typeList.Where(t =>
            {
                var attribute = t.GetCustomAttributes(typeof(DataRuleManageAttribute), false).FirstOrDefault();
                return attribute != null;
            });
            return result.ToList();
        }

        /// <summary>
        /// 根据属性名取得排序表达式
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static Expression<Func<TEntity, object>> GetOrder<TEntity>(string propertyName)
        {
            var property = typeof(TEntity).GetProperty(propertyName);
            return e => property.GetValue(e);
        }

    }
}
