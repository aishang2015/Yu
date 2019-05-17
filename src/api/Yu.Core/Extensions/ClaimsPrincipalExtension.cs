using System.Linq;
using System.Security.Claims;

namespace Yu.Core.Extensions
{
    /// <summary>
    /// ClaimsPrincipal的扩展
    /// </summary>
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// 根据claim的类型获取claim的值
        /// </summary>
        /// <param name="claimsPrincipal">用户的ClaimsPrincipal</param>
        /// <param name="claimType">声明的类型</param>
        /// <returns>声明的值</returns>
        public static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim == null ? string.Empty : claim.Value;
        }
    }
}
