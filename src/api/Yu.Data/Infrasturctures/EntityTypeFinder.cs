using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<Type> FindEntityTypes(Type dbcontextType)
        {
            // 寻找entity
            var typeList = typeof(BaseEntity<>).GetAllChildType();
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
}
