using System;
using System.Collections.Generic;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Repositories;
using ApiEntity = Yu.Data.Entities.Right.Api;

namespace Yu.Service.WebAdmin.Api
{
    public class ApiService : IApiService
    {
        // API仓储类
        private IRepository<ApiEntity, Guid> _apiRepository;

        // 构造函数
        public ApiService(IRepository<ApiEntity, Guid> apiRepository)
        {
            _apiRepository = apiRepository;
        }

        /// <summary>
        /// 取得API数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>API数据</returns>
        public PagedData<ApiEntity> GetApis(int pageIndex, int pageSize, string searchText)
        {
            var group = new ExpressionGroup<ApiEntity>()
            {
                TupleList = new List<(string, object, ExpressionType)> { ("Name", searchText, ExpressionType.StringContain), },
                ExpressionCombineType = ExpressionCombineType.And
            };

            // 查询过滤
            var query = _apiRepository.GetByWhere(group.GetLambda());

            // 生成结果
            return _apiRepository.GetByPage(query, pageIndex, pageSize);
        }
    }
}
