using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.WebAdmin.Group.InputModels;
using Yu.Service.WebAdmin.Group;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("组织结构管理")]
    public class GroupController : AuthorizeController
    {
        private IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        /// <summary>
        /// 取得所有组织
        /// </summary>
        [HttpGet("group")]
        [Description("取得所有组织")]
        public IActionResult GetAllGroup()
        {
            return Ok(_groupService.GetAllGroups());
        }

        /// <summary>
        /// 删除组织
        /// </summary>
        [HttpDelete("group")]
        [Description("删除组织")]
        public async Task<IActionResult> DeleteGroup([FromQuery]IdQuery idQuery)
        {
            await _groupService.DeleteGroup(idQuery.Id);
            return Ok();
        }

        /// <summary>
        /// 创建组织
        /// </summary>
        [HttpPost("group")]
        [Description("创建组织")]
        public async Task<IActionResult> AddGroup(GroupDetail groupDetail)
        {
            await _groupService.CreateGroup(groupDetail);
            return Ok();
        }

        /// <summary>
        /// 更新组织
        /// </summary>
        [HttpPut("group")]
        [Description("更新组织")]
        public async Task<IActionResult> UpdateGroup(GroupDetail groupDetail)
        {
            await _groupService.UpdateGroup(groupDetail);
            return Ok();
        }
    }
}