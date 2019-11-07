using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Yu.Data.Infrasturctures.BaseIdentity.Pemission
{
    public interface IPermissionCacheService
    {

        /// <summary>
        /// 获取角色的Claim
        /// </summary>
        /// <param name="roleName">角色名</param>
        /// <param name="roleName">Claim类型</param>
        Task<List<string>> GetRoleClaimValuesAsync(string roleName, string claimType);

        /// <summary>
        /// 判断当前用户是否有该api权限
        /// </summary>
        Task<bool> HaveApiRight(string address, string type);

        /// <summary>
        /// 取得角色拥有的前端识别
        /// </summary>
        Task<List<string>> GetRoleIdentificationAsync(List<string> roles);

        /// <summary>
        /// 取得角色拥有的前端路由
        /// </summary>
        Task<List<string>> GetRoleRoutesAsync(List<string> roles);

        /// <summary>
        /// 取得用户的数据规则
        /// </summary>
        Task<List<string>> GetRuleAsync(string dbContextName, string entityName);

        // 清除缓存操作
        void ClearRoleClaimCache(string roleName);
        void ClearElementCache();
        void ClearElementApiCache();
        void ClearApiCache();
    }
}
