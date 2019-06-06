using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Model.WebAdmin.User.OutputModels;

namespace Yu.Service.WebAdmin
{
    public interface IUserService
    {
        /// <summary>
        /// 取得用户概要数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>用户数据</returns>
        PagedData<UserOutline> GetUserOutlines(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户数据</returns>
        Task<UserDetail> GetUserDetail(Guid userId);

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userDetail">用户信息</param>
        Task UpdateUserDetail(UserDetail userDetail);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        Task DeleteUser(Guid userId);

    }
}
