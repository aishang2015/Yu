using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Data.Infrasturctures.BaseIdentity.Mvc;
using Yu.Model.Common.InputModels;
using Yu.Model.Message;
using Yu.Model.WebAdmin.User.OutputModels;
using Yu.Service.WebAdmin.User;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    [Description("用户管理")]
    public class UserController : ApiAuthorizeController
    {
        private readonly ILogger<UserController> _logger;

        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// 取得用户概要情报
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        [HttpGet("userOutline")]
        [Description("取得用户概要情报")]
        public async Task<IActionResult> GetUserOutlines([FromQuery]PagedQuery query)
        {
            // 取得数据            
            var result = await _userService.GetUserOutlinesAsync(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("userDetail")]
        [Description("取得用户数据")]
        public async Task<IActionResult> GetUserDetail([FromQuery]IdQuery query)
        {
            var user = await _userService.GetUserDetailAsync(query.Id);
            return Ok(user);
        }

        [HttpPost("userDetail")]
        [Description("添加新用户")]
        public async Task<IActionResult> AddNewUser([FromBody]UserDetail userDetail)
        {
            var result = await _userService.AddUserAsync(userDetail);
            if (result)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError("User", ErrorMessages.WebAdmin_User_E002);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// 更新用户数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPut("userDetail")]
        [Description("更新用户数据")]
        public async Task<IActionResult> UpdateUserDetail([FromBody]UserDetail query)
        {
            var result = await _userService.UpdateUserDetailAsync(query);
            if (result)
            {
                return Ok();
            }
            else
            {
                ModelState.AddModelError("User", ErrorMessages.WebAdmin_User_E001);
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// 删除用户数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpDelete("userDetail")]
        [Description("删除用户数据")]
        public async Task<IActionResult> DeleteUser([FromQuery]IdQuery query)
        {
            await _userService.DeleteUserAsync(query.Id);
            return Ok();
        }

        /// <summary>
        /// 设置用户头像
        /// </summary>
        /// <returns></returns>
        [HttpPost("userAvatar")]
        [Description("设置用户头像")]
        public async Task<IActionResult> SetUserAvatar([FromQuery]Guid userId, [FromForm]IFormFile file)
        {
            await _userService.UpdateUserAvatarAsync(userId, Request.Form.Files[0]);
            return Ok();
        }

    }
}