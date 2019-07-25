using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Extensions;
using Yu.Model.Account.InputModels;
using Yu.Model.Message;
using Yu.Service.Account;
using Yu.Service.WebAdmin.User;

namespace Yu.Api.Controllers
{
    [Description("个人信息")]
    [Route("api")]
    [ApiController]
    [Authorize]
    public class UserInfoController : ControllerBase
    {

        private readonly IUserService _userService;

        private readonly IAccountService _accountService;

        public UserInfoController(IUserService userService, IAccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        /// <summary>
        /// 设置用户头像
        /// </summary>
        /// <returns></returns>
        [HttpPost("userInfo/avatar")]
        [Description("设置用户头像")]
        public async Task<IActionResult> SetUserAvatar([FromForm]IFormFile file)
        {
            var userName = User.GetUserName();
            var avatarUri = await _userService.UpdateUserAvatar(userName, Request.Form.Files[0]);
            return Ok(new { avatar = avatarUri });
        }

        [HttpGet("userInfo")]
        [Description("取得用户信息")]
        public async Task<IActionResult> GetUserDetail()
        {
            var userName = User.GetUserName();
            var userDetail = await _userService.GetUserDetail(userName);
            return Ok(userDetail);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        [HttpPost("userInfo/password")]
        [Description("修改密码")]
        public async Task<IActionResult> ChangePwd([FromBody]ChangePwdModel model)
        {
            var userName = User.GetUserName();
            var result = await _accountService.ChangePassword(userName, model.OldPassword, model.NewPassword);
            if (!result)
            {
                ModelState.AddModelError("Password", ErrorMessages.Account_E008);
                return BadRequest(ModelState);
            }
            return Ok();
        }
    }
}