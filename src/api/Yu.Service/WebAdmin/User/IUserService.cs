using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Service.WebAdmin.User
{
    public interface IUserService
    {
        /// <summary>
        /// 取得用户概要数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>用户数据</returns>
        Task<PagedData<UserOutline>> GetUserOutlinesAsync(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户数据</returns>
        Task<UserDetail> GetUserDetailAsync(Guid userId);

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns>用户数据</returns>
        Task<UserDetail> GetUserDetailAsync(string userName);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDetail">用户信息</param>
        Task<bool> UpdateUserDetailAsync(UserDetail userDetail);

        /// <summary>
        /// 添加用户
        /// </summary>
        Task<bool> AddUserAsync(UserDetail userDetail);

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="formFile">表单头像文件</param>
        /// <returns></returns>
        Task<string> UpdateUserAvatarAsync(Guid userId, IFormFile formFile);

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="useName">用户名</param>
        /// <param name="formFile">表单头像文件</param>
        /// <returns></returns>
        Task<string> UpdateUserAvatarAsync(string userName, IFormFile formFile);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task DeleteUserAsync(Guid userId);

        /// <summary>
        /// 取得指定用户的角色
        /// </summary>
        Task<List<string>> GetUserRolesAsync(BaseIdentityUser baseIdentityUser);

    }
}
