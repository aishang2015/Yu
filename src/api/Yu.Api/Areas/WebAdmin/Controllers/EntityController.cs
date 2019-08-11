using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Service.WebAdmin.Entity;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("实体管理")]
    public class EntityController : ApiAuthorizeController
    {
        private readonly IEntityService _entityService;

        public EntityController(IEntityService entityService)
        {
            _entityService = entityService;
        }

        [Description("取得实体数据(下拉框用)")]
        [HttpGet("entities")]
        public IActionResult GetEntityInfo()
        {
            var result = _entityService.GetAllEntityOutline();
            return Ok(result);
        }

        [Description("取得实体数据")]
        [HttpGet("entity")]
        public IActionResult GetEntity([FromQuery]PagedQuery query)
        {
            var result = _entityService.GetEntities(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        [Description("添加实体数据")]
        [HttpPost("entity")]
        public async Task<IActionResult> AddEntity([FromBody]Entity entity)
        {
            await _entityService.InsertEntityAsync(entity);
            return Ok();

        }

        [Description("更新实体数据")]
        [HttpPut("entity")]
        public async Task<IActionResult> UpdateEntity([FromBody]Entity entity)
        {
            await _entityService.UpdateEntityAsync(entity);
            return Ok();

        }

        [Description("删除实体数据")]
        [HttpDelete("entity")]
        public async Task<IActionResult> DeleteEntity([FromQuery]IdQuery query)
        {
            await _entityService.DeleteEntityAsync(query.Id);
            return Ok();
        }
    }
}