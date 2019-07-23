using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Yu.Core.Expressions;

namespace Yu.Core.Utils
{
    public static class ReflectionUtil
    {

        /// <summary>
        /// 获取全部接口类型
        /// </summary>
        /// <param name="assemblyName">指定程序集</param>
        /// <returns>接口类型</returns>
        public static List<Type> GetInterfaces(string assemblyName)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 指定程序集的包
            var serviceLib = libs.Where(c => c.Assemblies.Contains(assemblyName)).FirstOrDefault();

            // 获取全部接口类型
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(serviceLib.Name));
            return assembly.GetTypes().Where(x => x.IsInterface).ToList();
        }


        /// <summary>
        /// 获取全部类类型
        /// </summary>
        /// <param name="assemblyName">指定程序集</param>
        /// <returns>类类型</returns>
        public static List<Type> GetClasses(string assemblyName)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 指定程序集的包
            var serviceLib = libs.Where(c => c.Assemblies.Contains(assemblyName)).FirstOrDefault();

            // 获取全部接口类型
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(serviceLib.Name));
            return assembly.GetTypes().Where(x => x.IsClass).ToList();
        }

        /// <summary>
        /// 获取当前程序全部的自定义程序集
        /// </summary>
        /// <param name="assemblyName">指定程序集</param>
        /// <returns>程序集列表</returns>
        public static List<Assembly> GetAssemblies()
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package");

            // 结果集合
            var result = new List<Assembly>();

            // 循环包数据
            foreach (var lib in libs)
            {
                result.Add(AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name)));
            }

            return result;
        }

        /// <summary>
        /// 简单类型转换
        /// </summary>
        /// <param name="o"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static object ConvertToType(object o, ExpressionType type, Type t)
        {
            if (type == ExpressionType.ListContain || type == ExpressionType.ListNotContain)
            {
                return o;
            }

            if (t == typeof(string))
            {
                return o.ToString();
            }
            else if (t.IsEnum)
            {
                return Enum.Parse(t, o.ToString());
            }
            else
            {
                var method = t.GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null)
                {
                    var result = method.Invoke(null, new object[] { o.ToString() });
                    return result;
                }
            }
            return o;
        }
    }
}
