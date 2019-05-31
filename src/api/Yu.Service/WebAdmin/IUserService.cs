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
    }
}
