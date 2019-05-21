using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Yu.Core.Extensions;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 仓储类相关的扩展
    /// </summary>
    public static class RepositoryServiceCollectionExtension
    {
        /// <summary>
        /// 服务内添加仓储合集
        /// </summary>
        /// <param name="services"></param>
        public static void AddRepositories(this IServiceCollection services)
        {
            // 所有对baseEntity的实现
            var typeList = typeof(BaseEntity<>).GetAllChildType();

            // 循环注入
            foreach (var type in typeList)
            {
                var genericType = type.GetBaseGenericArguments().FirstOrDefault();

                // IRepository 的类型
                var interfaceRepository = typeof(IRepository<,>).MakeGenericType(type, genericType);

                // BaseRepository 的类型
                var repository = typeof(BaseRepository<,>).MakeGenericType(type, genericType);
                services.AddScoped(interfaceRepository, repository);
            }
        }


    }
}
