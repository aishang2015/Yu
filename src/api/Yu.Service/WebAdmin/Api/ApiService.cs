using System;
using System.Collections.Generic;
using Yu.Core.Utils;
using Yu.Data.Entities;
using Yu.Data.Repositories;
using ExpressionType = Yu.Core.Utils.ExpressionType;

namespace Yu.Service.WebAdmin.Api
{
    public class ApiService : IApiService
    {
        // API仓储类
        private IRepository<Data.Entities.Right.Api, Guid> _apiRepository;

        // 构造函数
        public ApiService(IRepository<Data.Entities.Right.Api, Guid> apiRepository)
        {
            _apiRepository = apiRepository;
        }

        /// <summary>
        /// 取得API数据
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns>API数据</returns>
        public PagedData<Data.Entities.Right.Api> GetApis(int pageIndex, int pageSize, string searchText)
        {
            // 生成表达式组
            var expressions = ExpressionUtil<Data.Entities.Right.Api>.GetExpressions(new List<(string, object, ExpressionType)>
            {
                ("Name",searchText,ExpressionType.StringContain),
            });

            // 组合表达式组
            var expression = ExpressionUtil<Data.Entities.Right.Api>.CombinExpressions(expressions, ExpressionCombineType.And);

            // 生成条件
            var filter = ExpressionUtil<Data.Entities.Right.Api>.GetLambda(expression);

            // 查询过滤
            var query = _apiRepository.GetByWhere(filter);

            // 生成结果
            return _apiRepository.GetByPage(query, pageIndex, pageSize);
        }
    }
}
