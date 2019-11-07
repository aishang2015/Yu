using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Extensions;
using Yu.Data.Infrasturctures.BaseIdentity.Pemission;

namespace Yu.Data.Infrasturctures.BaseIdentity.Mvc
{

    // 由于要访问roleservice临时将文件放在这里
    public class ApiAuthorizationHandler : AuthorizationHandler<ApiAuthorizationRequirement>
    {
        private readonly IMemoryCache _memoryCache;

        private readonly IPermissionCacheService _permissionCacheService;

        public ApiAuthorizationHandler(IMemoryCache memoryCache,
            IPermissionCacheService permissionCacheService)
        {
            _memoryCache = memoryCache;
            _permissionCacheService = permissionCacheService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ApiAuthorizationRequirement requirement)
        {
            // 转化为HTTPCONTEXT
            AuthorizationFilterContext filterContext = context.Resource as AuthorizationFilterContext;
            HttpContext httpContext = filterContext.HttpContext;

            // 取得请求路径和请求方法
            var requestPath = httpContext.Request.Path.Value;
            var requestMethod = httpContext.Request.Method;

            // 取得用户角色
            var checkResult = await _permissionCacheService.HaveApiRight(requestPath, requestMethod);

            if (checkResult)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

        }
    }
}
