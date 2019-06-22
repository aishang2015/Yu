using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.WebAdmin.Element.InputModels;
using Yu.Service.WebAdmin.Element;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("页面元素管理")]
    public class ElementController : AuthorizeController
    {
        private readonly IElementService _elementService;

        public ElementController(IElementService elementService)
        {
            _elementService = elementService;
        }

        /// <summary>
        /// 取得页面元素
        /// </summary>
        /// <returns></returns>
        [Description("取得页面元素")]
        [HttpGet("element")]
        public IActionResult GetAllElements()
        {
            var result = _elementService.GetAllElement();
            return Ok(result);
        }

        /// <summary>
        /// 添加新元素
        /// </summary>
        /// <returns></returns>
        [Description("添加新元素")]
        [HttpPost("element")]
        public async Task<IActionResult> AddElement([FromBody]ElementDetail elementDetail)
        {
            await _elementService.CreateElement(elementDetail);
            return Ok();
        }

        /// <summary>
        /// 添加新元素
        /// </summary>
        /// <returns></returns>
        [Description("删除元素")]
        [HttpPost("element")]
        public IActionResult DeleteElement([FromBody]ElementDetail elementDetail)
        {            
            return Ok();
        }
    }
}