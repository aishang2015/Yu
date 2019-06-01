using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Yu.Core.Extensions;
using Yu.Core.Utils;
using Yu.Data.Entities;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 配置entity以及configuration
    /// </summary>
    public static class ModelBuilderExtension
    {
        /// <summary>
        /// 配置entity以及configuration
        /// </summary>
        /// <param name="builder">ModelBuilder</param>
        /// <param name="dbContextType">数据库类型</param>
        public static void SetEntityConfiguration(this ModelBuilder builder, Type dbContextType)
        {
            // 寻找entity
            var entityTypeList = EntityTypeFinder.FindEntityTypes(dbContextType);
            entityTypeList.ForEach(type => builder.Entity(type));

            // 寻找entity的configuration
            var assemblies = ReflectionUtil.GetAssemblies();
            foreach (var assemliy in assemblies)
            {
                // 从程序及载入configuration
                builder.ApplyConfigurationsFromAssembly(assemliy, type =>
                {
                    // 类型判断
                    if (type.HasImplementedRawGeneric(typeof(IEntityTypeConfiguration<>)))
                    {
                        // 取得IEntityTypeConfiguration泛型参数类型和entity类型进行对比
                        var entityType = type.GetInterfaces()[0].GetGenericArguments()[0];
                        return entityTypeList.Contains(entityType);
                    }
                    return false;
                });
            }
        }



    }
}
