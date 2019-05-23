using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Yu.Core.Extensions;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 仓储类相关的扩展
    /// </summary>
    public static class RepositoryServiceCollectionExtension
    {
        /// <summary>
        /// 添加仓储合集
        /// </summary>
        /// <param name="services"></param>
        public static void AddRepositories<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            // 所有对baseEntity的实现
            var typeList = EntityTypeFinder.FindEntityTypes(typeof(TDbContext));

            // 循环注入
            foreach (var type in typeList)
            {
                var genericType = type.GetBaseGenericArguments().FirstOrDefault();

                // IRepository 的类型
                var interfaceRepository = typeof(IRepository<,>).MakeGenericType(type, genericType);

                // BaseRepository 的类型
                var repository = typeof(BaseRepository<,,>).MakeGenericType(type, genericType, typeof(TDbContext));
                services.AddScoped(interfaceRepository, repository);
            }

            // 工作单元
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        /// <summary>
        /// 添加读写分离仓储
        /// </summary>
        /// <param name="services"></param>
        public static void AddRWSplittingRepositories<TReadDbContext, TWriteDbContext>(this IServiceCollection services)
            where TReadDbContext : DbContext
            where TWriteDbContext : DbContext
        {
            // 所有对baseEntity的实现
            var typeList = EntityTypeFinder.FindEntityTypes(typeof(TReadDbContext));

            // 循环注入
            foreach (var type in typeList)
            {
                var genericType = type.GetBaseGenericArguments().FirstOrDefault();

                // IRepository 的类型
                var interfaceRepository = typeof(IRepository<,>).MakeGenericType(type, genericType);

                // RWSplittingRepository 的类型
                var repository = typeof(RWSplittingRepository<,,,>).MakeGenericType(type, genericType, typeof(TReadDbContext), typeof(TWriteDbContext));
                services.AddScoped(interfaceRepository, repository);
            }

            // 工作单元
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }


    }
}
