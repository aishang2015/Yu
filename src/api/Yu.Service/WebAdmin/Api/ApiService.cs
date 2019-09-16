using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Infrasturctures.BaseIdentity;
using Yu.Data.Repositories;
using ApiEntity = Yu.Data.Entities.Right.Api;

namespace Yu.Service.WebAdmin.Api
{
    public class ApiService : IApiService
    {
        // API仓储类
        private IRepository<ApiEntity, Guid> _apiRepository;

        // 工作单元
        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        public ApiService(IRepository<ApiEntity, Guid> apiRepository, IUnitOfWork<BaseIdentityDbContext> unitOfWork)
        {
            _apiRepository = apiRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// 添加api数据
        /// </summary>
        /// <param name="api">api数据</param>
        public async Task AddApiAsync(ApiEntity api)
        {
            await _apiRepository.InsertAsync(api);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 删除api数据
        /// </summary>
        /// <param name="apiId">api的id</param>
        public async Task DeleteApiAsync(Guid apiId)
        {
            _apiRepository.DeleteRange(api => api.Id == apiId);
            await _unitOfWork.CommitAsync();
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
            var query = _apiRepository.GetByWhereNoTracking(group.GetLambda());

            // 生成结果
            return _apiRepository.GetByPage(query, pageIndex, pageSize);
        }

        /// <summary>
        /// 更新api数据
        /// </summary>
        /// <param name="api">api数据</param>
        public async Task UpdateApiAsync(ApiEntity api)
        {
            _apiRepository.Update(api);
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        /// 全部api
        /// </summary>
        public IEnumerable<ApiEntity> GetAllApi()
        {
            return _apiRepository.GetAllNoTracking();
        }
    }
}
