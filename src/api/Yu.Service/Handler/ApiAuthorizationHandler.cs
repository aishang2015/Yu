using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Extensions;
using Yu.Service.WebAdmin.Role;

namespace Yu.Service.Handler
{

    // 由于要访问roleservice临时将文件放在这里
    public class ApiAuthorizationHandler : AuthorizationHandler<ApiAuthorizationRequirement>
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IRoleService _roleService;

        public ApiAuthorizationHandler(IMemoryCache memoryCache, IRoleService roleService)
        {
            _memoryCache = memoryCache;
            _roleService = roleService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizationRequirement requirement)
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
                return;
            }
            else
            {
                var api = requestPath + '|' + requestMethod;
                foreach (var role in roles)
                {
                    var permissions = await _roleService.GetRolePermission(role);
                    var apis = permissions.Where(tuple => tuple.Item1 == PermissionTypes.Apis).Select(tuple => tuple.Item2).FirstOrDefault().Split(',');
                    if (apis.Contains(api))
                    {
                        context.Succeed(requirement);
                        return;
                    }

                }

            }

            context.Fail();
        }
    }
}
