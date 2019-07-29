using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;
using Yu.Model.WebAdmin.Role.InputOuputModels;
using Yu.Service.WebAdmin.Role;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("角色管理")]
    public class RoleController : AuthorizeController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet("roleNames")]
        [Description("取得全部角色名称")]
        public IActionResult GetAllRoleName()
        {
            var data = _roleService.GetAllRoleNames();
            return Ok(data);
        }

        [HttpGet("roleOutline")]
        [Description("取得角色概要数据")]
        public IActionResult GetRoleOutline([FromQuery]PagedQuery query)
        {
            var data = _roleService.GetRole(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(data);
        }

        [HttpGet("role")]
        [Description("取得角色详细数据")]
        public async Task<IActionResult> GetRoleDetail([FromQuery]IdQuery query)
        {
            var data = await _roleService.GetRoleAsync(query.Id);
            return Ok(data);
        }

        [HttpDelete("role")]
        [Description("删除角色数据")]
        public async Task<IActionResult> DeleteRole([FromQuery]IdQuery query)
        {
            await _roleService.DeleteRoleAsync(query.Id);
            return Ok();
        }

        [HttpPost("role")]
        [Description("创建角色")]
        public async Task<IActionResult> AddRole([FromBody]RoleDetail role)
        {
            var result = await _roleService.AddRoleAsync(role);
            if (result)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError("RoleName", ErrorMessages.WebAdmin_Role_E001);
                return BadRequest(ModelState);
            }
        }

        [HttpPut("role")]
        [Description("更新角色")]
        public async Task<IActionResult> UpdateRole([FromBody]RoleDetail role)
        {
            await _roleService.UpdateRoleAsync(role);
            return Ok();
        }
    }
}