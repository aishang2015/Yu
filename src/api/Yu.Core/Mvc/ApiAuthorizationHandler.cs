using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Extensions;

namespace Yu.Core.Mvc
{
    public class ApiAuthorizationHandler : AuthorizationHandler<ApiAuthorizationRequirement>
    {
        private readonly IMemoryCache _memoryCache;

        public ApiAuthorizationHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizationRequirement requirement)
        {
            // 转化为HTTPCONTEXT
            AuthorizationFilterContext filterContext = context.Resource as AuthorizationFilterContext;
            HttpContext httpContext = filterContext.HttpContext;

            // 取得请求路径和请求方法
            var requestPath = httpContext.Request.Path.Value;
            var requestMethod = httpContext.Request.Method;

            var roles = context.User.GetClaimValue(CustomClaimTypes.Role).Split(',');

            if (roles.Contains("系统管理员"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            else
            {
                var api = requestPath + '|' + requestMethod;
                foreach (var role in roles)
                {
                    // 缓存中获取该角色的权限
                    var result = _memoryCache.TryGetValue(CommonConstants.RoleMemoryCacheKey + role, out List<(string, string)> permissions);

                    // 无法获取则直接处理失败
                    if (result)
                    {
                        var apis = permissions.Where(tuple => tuple.Item1 == PermissionTypes.Apis).Select(tuple => tuple.Item2).FirstOrDefault().Split(',');
                        if (apis.Contains(api))
                        {
                            context.Succeed(requirement);
                            return Task.CompletedTask; 
                        }
                    }

                }

            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
