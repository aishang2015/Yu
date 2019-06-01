using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Yu.Core.Extensions
{
    /// <summary>
    /// 反射类型扩展
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// 判断类型是否是某泛型类或泛型接口的实现
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="generic">父类型</param>
        /// <returns>是否继承于父类型</returns>
        public static bool HasImplementedRawGeneric(this Type type, Type genericType)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (genericType == null) throw new ArgumentNullException(nameof(genericType));

            // 接口判断
            foreach (var inteface in type.GetInterfaces())
            {
                if (IsTheRawGenericType(inteface)) return true;
            }

            // 在类型的基类内进行判断
            type = type.BaseType;
            while (type != null && type != typeof(object))
            {
                if (IsTheRawGenericType(type)) return true;
                type = type.BaseType;
            }

            // 没有找到任何匹配的接口或类型。
            return false;

            // 测试类型是否是泛型,并且获取泛型类型
            bool IsTheRawGenericType(Type test)
                => genericType == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }

        /// <summary>
        /// 获取当前类型的泛型参数类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>泛型参数的类型</returns>
        public static Type[] GetBaseGenericArguments(this Type type)
        {
            type = type.BaseType;
            while (type != null && type != typeof(object))
            {
                if (type.GetGenericArguments().Count() > 0) return type.GetGenericArguments();
                type = type.BaseType;
            }
            return null;
        }

        /// <summary>
        /// 在全部程序集范围内查找当前类的子类
        /// </summary>
        /// <param name="type">父类型</param>
        /// <param name="assemblyName">指定程序集</param>
        /// <returns>子类型</returns>        
        public static List<Type> GetAllChildType(this Type type, string assemblyName = null)
        {
            // 过滤系统包和nuget包
            var libs = DependencyContext.Default.CompileLibraries.Where(lib => !lib.Serviceable && lib.Type != "package").ToList();

            // 指定程序集不为空时
            if (!string.IsNullOrEmpty(assemblyName))
            {
                libs = libs.Where(c => c.Assemblies.Contains(assemblyName)).ToList();
            }

            // 指定程序集的包
            var typeList = new List<Type>();

            // 获取全部继承type的类型
            foreach (var lib in libs)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(lib.Name));
                typeList.AddRange(assembly.GetTypes().Where(x => x.HasImplementedRawGeneric(type)).ToList());
            }

            return typeList;
        }
    }
}
