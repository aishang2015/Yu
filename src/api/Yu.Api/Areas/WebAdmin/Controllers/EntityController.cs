using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Entities.Right;
using Yu.Model.WebAdmin.Entity.InputModels;
using Yu.Service.WebAdmin.Entity;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("实体管理")]
    public class EntityController : AuthorizeController
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
        public IActionResult GetEntity([FromQuery]EntityGetQuery query)
        {
            var result = _entityService.GetEntities(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        [Description("添加实体数据")]
        [HttpPost("entity")]
        public async Task<IActionResult> AddEntity([FromBody]Entity entity)
        {
            await _entityService.InsertEntity(entity);
            return Ok();

        }

        [Description("更新实体数据")]
        [HttpPut("entity")]
        public async Task<IActionResult> UpdateEntity([FromBody]Entity entity)
        {
            await _entityService.UpdateEntity(entity);
            return Ok();

        }

        [Description("删除实体数据")]
        [HttpDelete("entity")]
        public async Task<IActionResult> DeleteEntity([FromQuery]EntityDeleteQuery query)
        {
            await _entityService.DeleteEntity(query.EntityId);
            return Ok();
        }
    }
}