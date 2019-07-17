using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.Test.InputOuputModels;
using Yu.Service.Test.TestData;

namespace Yu.Api.Areas.Test.Controllers
{
    [Route("api")]
    [Description("测试业务数据")]
    public class TestDataController : AuthorizeController
    {
        private readonly ITestDataService _testDataService;

        public TestDataController(ITestDataService testDataService)
        {
            _testDataService = testDataService;
        }


        [HttpGet("testData")]
        [Description("取得业务数据")]
        public IActionResult GetTestData([FromQuery]PagedQuery query)
        {
            var result = _testDataService.GetTestData(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        [HttpPost("testData")]
        [Description("添加业务数据")]
        public async Task<IActionResult> AddTestData([FromBody]TestDataDetail testData)
        {
            await _testDataService.AddTestData(testData);
            return Ok();
        }

        [HttpPut("testData")]
        [Description("更新业务数据")]
        public async Task<IActionResult> UpdateTestData([FromBody]TestDataDetail testData)
        {
            await _testDataService.UpdateTestData(testData);
            return Ok();
        }

        [HttpDelete("testData")]
        [Description("删除业务数据")]
        public async Task<IActionResult> DeleteTestData([FromQuery]IdQuery query)
        {
            await _testDataService.DeleteTestData(query.Id);
            return Ok();
        }

    }
}