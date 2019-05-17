using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Yu.Core.Extensions
{
    /// <summary>
    /// 系统注入扩展
    /// </summary>
    public static class InjectionExtension
    {
        /// <summary>
        /// 批量注入（Scope）
        /// </summary>
        /// <param name="services">服务合集</param>
        /// <param name="assemblyName">程序集名称</param>
        public static void AddScopedBatch(this IServiceCollection services, string assemblyName)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 指定程序集的包
            var serviceLib = libs.Where(c => c.Name.Contains(assemblyName)).FirstOrDefault();

            // 获取全部接口类型
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(serviceLib.Name));
            var typeList = assembly.GetTypes().Where(x => x.IsInterface).ToList();

            // 假定接口名为IXXXX则类型名为XXXX
            foreach (var type in typeList)
            {
                // 查找接口的实现类
                var implementName = type.Name.Substring(1, type.Name.Length - 1);
                var implementType = assembly.GetTypes().Where(c => c.IsClass && c.Name == implementName).FirstOrDefault();
                if (implementType == null) continue;

                // 注入实现
                services.AddScoped(type, implementType);
            }
        }

        /// <summary>
        /// 批量注入（Singleton）
        /// </summary>
        /// <param name="services">服务合集</param>
        /// <param name="assemblyName">程序集名称</param>
        public static void AddSingletonBatch(this IServiceCollection services, string assemblyName)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 指定程序集的包
            var serviceLib = libs.Where(c => c.Name.Contains(assemblyName)).FirstOrDefault();

            // 获取全部接口类型
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(serviceLib.Name));
            var typeList = assembly.GetTypes().Where(x => x.IsInterface).ToList();

            // 假定接口名为IXXXX则类型名为XXXX
            foreach (var type in typeList)
            {
                // 查找接口的实现类
                var implementName = type.Name.Substring(1, type.Name.Length - 1);
                var implementType = assembly.GetTypes().Where(c => c.IsClass && c.Name == implementName).FirstOrDefault();
                if (implementType == null) continue;

                // 注入实现
                services.AddSingleton(type, implementType);
            }
        }

        /// <summary>
        /// 批量注入（Transient）
        /// </summary>
        /// <param name="services">服务合集</param>
        /// <param name="assemblyName">程序集名称</param>
        public static void AddTransientBatch(this IServiceCollection services, string assemblyName)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 指定程序集的包
            var serviceLib = libs.Where(c => c.Name.Contains(assemblyName)).FirstOrDefault();

            // 获取全部接口类型
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(serviceLib.Name));
            var typeList = assembly.GetTypes().Where(x => x.IsInterface).ToList();

            // 假定接口名为IXXXX则类型名为XXXX
            foreach (var type in typeList)
            {
                // 查找接口的实现类
                var implementName = type.Name.Substring(1, type.Name.Length - 1);
                var implementType = assembly.GetTypes().Where(c => c.IsClass && c.Name == implementName).FirstOrDefault();
                if (implementType == null) continue;

                // 注入实现
                services.AddTransient(type, implementType);
            }
        }
    }
}
