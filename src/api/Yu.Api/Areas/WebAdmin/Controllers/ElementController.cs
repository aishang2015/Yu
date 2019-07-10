using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;
using Yu.Model.WebAdmin.Element;
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
        [Description("添加新元素")]
        [HttpPost("element")]
        public async Task<IActionResult> AddElement([FromBody]ElementDetail elementDetail)
        {
            // 验证唯一识别
            var eles = _elementService.HaveSameIdentification(elementDetail.Identification);
            if (eles.Count() > 0)
            {
                ModelState.AddModelError("Identification",
                    string.Format(ErrorMessages.WebAdmin_Element_E004, string.Join(',', eles)));
                return BadRequest(ModelState);
            }

            await _elementService.CreateElement(elementDetail);
            return Ok();
        }

        /// <summary>
        /// 添加新元素
        /// </summary>
        [Description("删除元素")]
        [HttpDelete("element")]
        public async Task<IActionResult> DeleteElement([FromQuery]IdQuery elementQuery)
        {
            await _elementService.DeleteElement(elementQuery.Id);
            return Ok();
        }

        /// <summary>
        /// 更新元素
        /// </summary>
        [Description("更新元素")]
        [HttpPut("element")]
        public async Task<IActionResult> UpdateElement([FromBody]ElementDetail elementDetail)
        {
            // 验证唯一识别
            var eles = _elementService.HaveSameIdentification(Guid.Parse(elementDetail.Id), elementDetail.Identification);
            if (eles.Count() > 0)
            {
                ModelState.AddModelError("Identification",
                    string.Format(ErrorMessages.WebAdmin_Element_E004, string.Join(',', eles)));
                return BadRequest(ModelState);
            }

            await _elementService.UpdateElement(elementDetail);
            return Ok();
        }
    }
}