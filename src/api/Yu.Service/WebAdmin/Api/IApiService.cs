using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Data.Entities;

namespace Yu.Service.WebAdmin.Api
{
    public interface IApiService
    {

        /// <summary>
        /// 取得API数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>API数据</returns>
        PagedData<Data.Entities.Right.Api> GetApis(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 删除api数据
        /// </summary>
        /// <param name="apiId">api的id</param>
        Task DeleteApiAsync(Guid apiId);

        /// <summary>
        /// 添加api数据
        /// </summary>
        /// <param name="api">api数据</param>
        Task AddApiAsync(Data.Entities.Right.Api api);

        /// <summary>
        /// 更新api数据
        /// </summary>
        /// <param name="api">api数据</param>
        Task UpdateApiAsync(Data.Entities.Right.Api api);

        /// <summary>
        /// 全部api
        /// </summary>
        IEnumerable<Data.Entities.Right.Api> GetAllApi();
    }
}
