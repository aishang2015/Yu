using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Yu.Data.Infrasturctures.Pemission
{
    public interface IPermissionCacheService
    {
        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        /// <param name="roleName">角色名</param>
        Task<List<Claim>> GetRoleClaimAsync(string roleName);

        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <param name="roleName">Claim类型</param>
        Task<List<string>> GetRoleClaimValuesAsync(string roleName, string claimType);

        /// <summary>
        /// 取得角色的api权限
        /// </summary>
        /// <returns></returns>
        Task<string> GetRoleApiAsync(string roleName);

        /// <summary>
        /// 取得角色拥有的前端识别
        /// </summary>
        /// <param name="roleName">角色名称</param>
        Task<string> GetRoleIdentificationAsync(string roleName);

        /// <summary>
        /// 取得角色拥有的前端路由
        /// </summary>
        /// <param name="roleName">角色名称</param>
        Task<string> GetRoleRoutesAsync(string roleName);

        /// <summary>
        /// 清除角色的权限缓存
        /// </summary>
        /// <param name="roleName">角色名称</param>
        Task ClearRolePermissionCache(string roleName);

        /// <summary>
        /// 清除所有角色的权限缓存
        /// </summary>
        Task ClearAllRolePermissionCache();

        /// <summary>
        /// 更新角色拥有的所有权限的缓存
        /// </summary>
        Task<List<string>> GetUserRuleAsync(string userName,string[] userRoles);
    }
}
