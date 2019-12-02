using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Yu.Core.Mvc
{
    public static class MvcApplicationBuilderExtension
    {
        /// <summary>
        /// 获取全部api信息
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>列表（api描述，api所属控制器，http类型，api地址）</returns>
        public static List<(string, string, string, string)> GetApiInfos(this IHost host)
        {
            var result = new List<(string, string, string, string)>();

            var _partManager = host.Services.GetRequiredService<ApplicationPartManager>();

            var controllerFeature = new ControllerFeature();

            // 填充controllerFeature
            _partManager.PopulateFeature(controllerFeature);

            // 控制器合集
            var controllerTypes = controllerFeature.Controllers.Where(c => !c.IsAbstract).ToList();

            foreach (var typeInfo in controllerTypes)
            {
                // 控制器描述
                var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(typeInfo, typeof(DescriptionAttribute));

                // 控制器路由
                var routeAttribute = (RouteAttribute)(Attribute.GetCustomAttributes(typeInfo, typeof(RouteAttribute)).FirstOrDefault());

                // 控制器名称
                var controllerName = typeInfo.Name.Replace("Controller", string.Empty);

                // 动作合集
                var actionInfos = typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                foreach (var methodInfo in actionInfos)
                {
                    // 动作的描述
                    var methoddescriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(DescriptionAttribute));

                    // 动作的请求类型
                    var httpAttribute = (HttpMethodAttribute)Attribute.GetCustomAttribute(methodInfo, typeof(HttpMethodAttribute));

                    // 最终路径 
                    // 这里约定两种情况“api/[controller]/[action]”即带action，httpattribute不带参数使用默认的方法名
                    // 和不带action但是httpattribute必须带参数。
                    var finnalPath = routeAttribute.Template.Replace("[controller]", controllerName).Replace("[action]", methodInfo.Name);
                    finnalPath = '/' + (string.IsNullOrEmpty(httpAttribute.Template) ? finnalPath : finnalPath) + '/' + httpAttribute.Template;

                    result.Add((descriptionAttribute?.Description + '-' + methoddescriptionAttribute?.Description, controllerName, httpAttribute.HttpMethods.First(), finnalPath));
                }
            }
            return result;
        }
    }
}
