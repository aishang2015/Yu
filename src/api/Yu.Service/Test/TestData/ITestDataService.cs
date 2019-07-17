using System;
using System.Threading.Tasks;
using Yu.Data.Entities;
using Yu.Model.Test.InputOuputModels;

namespace Yu.Service.Test.TestData
{
    public interface ITestDataService
    {
        /// <summary>
        /// 获取测试业务数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        PagedData<TestDataDetail> GetTestData(int pageIndex, int pageSize, string searchText);

        /// <summary>
        /// 添加测试业务数据
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        Task AddTestData(TestDataDetail testData);

        /// <summary>
        /// 删除测试业务数据
        /// </summary>
        /// <returns></returns>
        Task DeleteTestData(Guid id);

        /// <summary>
        /// 更新测试业务数据
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        Task UpdateTestData(TestDataDetail testData);

    }
}
