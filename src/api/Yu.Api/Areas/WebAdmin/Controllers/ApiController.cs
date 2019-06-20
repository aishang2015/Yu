using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
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

    }
}