using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Model.WebAdmin.Role.InputOuputModels;
using Yu.Model.WebAdmin.Role.OutputModels;

namespace Yu.Service.WebAdmin.Role
{
    public interface IRoleService
    {
        /// <summary>
        /// 取得全部角色名
        /// </summary>
        string[] GetAllRoleNames();

        /// <summary>
        /// 取得角色概要
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="searchText">搜索内容</param>
        PagedData<RoleOutline> GetRole(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role">角色</param>
        Task<bool> AddRoleAsync(RoleDetail role);

        /// <summary>
        /// 取得角色
        /// </summary>
        /// <param name="id">角色id</param>
        Task<RoleDetail> GetRoleAsync(Guid id);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id</param>
        Task DeleteRoleAsync(Guid id);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="role">角色</param>
        Task UpdateRoleAsync(RoleDetail role);


        /// <summary>
        /// 取得角色拥有的所有权限
        /// </summary>
        /// <param name="roleName">角色名称</param>
        Task<Dictionary<string, string>> GetRolePermissionAsync(string roleName);

        /// <summary>
        /// 更新角色拥有的所有权限的缓存
        /// </summary>
        /// <param name="roleName">角色名称</param>
        Task<Dictionary<string, string>> UpdateRolePermissionCacheAsync(string roleName);

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
        Task<List<string>> GetRoleClaimAsync(string roleName, string claimType);

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
    }
}
