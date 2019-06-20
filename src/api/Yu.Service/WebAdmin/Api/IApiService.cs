using Microsoft.AspNetCore.Identity;
using System;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;

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
    }
}
