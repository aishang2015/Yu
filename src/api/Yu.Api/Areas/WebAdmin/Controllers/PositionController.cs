
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities;
using Yu.Model.Common.InputModels;
using Yu.Data.Entities.WebAdmin;
using Yu.Service.WebAdmin.Positions;
using Yu.Model.Message;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("岗位管理")]
    public class PositionController : AuthorizeController
    {
        private readonly IPositionService _service;

        public PositionController(IPositionService service)
        {
            _service = service;
        }

		/// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("position")]
        [Description("取得岗位数据")]
        public IActionResult GetPositions([FromQuery] PagedQuery query)
        {
            var result = _service.GetPositions(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 取得数据
        /// </summary>
        [HttpGet("positions")]
        [Description("取得岗位数据")]
        public IActionResult GetAllPositions()
        {
            var result = _service.GetAllPositions();
            return Ok(result);
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        [HttpPost("position")]
        [Description("添加岗位数据")]
        public async Task<IActionResult> AddPosition([FromBody]Position entity)
        {
            var result = _service.HaveRepeatName(entity.Id, entity.PositionName);
            if (result)
            {
                ModelState.AddModelError("Name", ErrorMessages.WebAdmin_Position_E001);
                return BadRequest(ModelState);
            }

            await _service.AddPositionAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        [HttpPut("position")]
        [Description("更新岗位数据")]
        public async Task<IActionResult> UpdatePosition([FromBody]Position entity)
        {
            var result = _service.HaveRepeatName(entity.Id, entity.PositionName);
            if (result)
            {
                ModelState.AddModelError("Name", ErrorMessages.WebAdmin_Position_E001);
                return BadRequest(ModelState);
            }

            await _service.UpdatePositionAsync(entity);
            return Ok();
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        [HttpDelete("position")]
        [Description("删除岗位数据")]
        public async Task<IActionResult> DeletePosition([FromQuery]IdQuery query)
        {
            await _service.DeletePositionAsync(query.Id);
            return Ok();
        }
	}
}

