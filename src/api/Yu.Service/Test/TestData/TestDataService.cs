using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Expressions;
using Yu.Data.Entities;
using Yu.Data.Entities.Test;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;
using Yu.Model.Test.InputOuputModels;
using TestDataEntity = Yu.Data.Entities.Test.TestData;

namespace Yu.Service.Test.TestData
{
    public class TestDataService : ITestDataService
    {
        private readonly IRepository<TestDataEntity, Guid> _testDataRepository;

        private readonly IUnitOfWork<BaseIdentityDbContext> _unitOfWork;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TestDataService(IRepository<TestDataEntity, Guid> testDataRepository,
            IUnitOfWork<BaseIdentityDbContext> unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _testDataRepository = testDataRepository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddTestData(TestDataDetail testData)
        {
            var data = Mapper.Map<TestDataEntity>(testData);

            data.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserName).Value;
            data.UserGroupId =  _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Group).Value;

            await _testDataRepository.InsertAsync(data);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteTestData(Guid id )
        {
            _testDataRepository.DeleteRange(e => e.Id == id);
            await _unitOfWork.CommitAsync();
        }

        public PagedData<TestDataDetail> GetTestData(int pageIndex, int pageSize, string searchText)
        {
            var expressionGroup = new ExpressionGroup<TestDataEntity>()
            {
                ExpressionCombineType = ExpressionCombineType.Or,
                TupleList = new List<(string, object, ExpressionType)>
                  {
                      ("Field1",searchText,ExpressionType.StringContain),
                      ("Field2",searchText,ExpressionType.StringContain),
                      ("Field3",searchText,ExpressionType.StringContain),
                  }
            };

            var filter = expressionGroup.GetLambda();

            var query = _testDataRepository.GetByWhereNoTracking(filter);
            var testDatas = _testDataRepository.GetByPage(query, pageIndex, pageSize);
            return new PagedData<TestDataDetail>
            {
                Data = Mapper.Map<List<TestDataDetail>>(testDatas.Data),
                Total = query.Count()
            };
        }

        public async Task UpdateTestData(TestDataDetail testData)
        {
            var data = Mapper.Map<TestDataEntity>(testData);
            data.UserName = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserName).Value;
            data.UserGroupId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Group).Value;

            _testDataRepository.Update(data);
            await _unitOfWork.CommitAsync();
        }
    }
}
