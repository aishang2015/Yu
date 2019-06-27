using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Yu.Core.Mvc;
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
    }
}