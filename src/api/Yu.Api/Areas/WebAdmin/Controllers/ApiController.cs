using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.WebAdmin.Api.InputModels;
using Yu.Service.WebAdmin.Api;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("接口管理")]
    public class ApiController : AuthorizeController
    {
        private readonly IApiService _apiService;

        public ApiController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet("allApi")]
        [Description("取得全部Api数据")]
        public IActionResult GetAllApis()
        {
            return Ok(_apiService.GetAllApi());
        }

        /// <summary>
        /// 取得API数据
        /// </summary>
        /// <param name="query">请求参数</param>
        /// <returns>API数据</returns>
        [HttpGet("api")]
        [Description("取得API数据")]
        public IActionResult GetApis([FromQuery] ApiQuery query)
        {
            var result = _apiService.GetApis(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 创建Api数据
        /// </summary>
        /// <param name="api">api数据</param>
        [HttpPost("api")]
        [Description("添加API数据")]
        public async Task<IActionResult> AddApi([FromBody]Yu.Data.Entities.Right.Api api)
        {
            await _apiService.AddApi(api);
            return Ok();
        }

        /// <summary>
        /// 更新api数据
        /// </summary>
        /// <param name="api">api数据</param>
        [HttpPut("api")]
        [Description("更新api数据")]
        public async Task<IActionResult> UpdateApi([FromBody]Data.Entities.Right.Api api)
        {
            await _apiService.UpdateApi(api);
            return Ok();
        }

        /// <summary>
        /// 删除api数据
        /// </summary>
        /// <param name="query">apiId</param>
        [HttpDelete("api")]
        [Description("删除api数据")]
        public async Task<IActionResult> DeleteApi([FromQuery]ApiDeleteQuery query)
        {
            await _apiService.DeleteApi(query.ApiId);
            return Ok();
        }

    }
}