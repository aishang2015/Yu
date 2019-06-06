using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Yu.Core.Mvc;
using Yu.Model.WebAdmin.User.InputModels;
using Yu.Model.WebAdmin.User.OutputModels;
using Yu.Service.WebAdmin;

namespace Yu.Api.Areas.WebAdmin.Controllers
{
    [Route("api")]
    public class UserController : AnonymousController
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
        public IActionResult GetUserOutlines([FromQuery]UserOutlineQuery query)
        {
            // 取得数据            
            var result = _userService.GetUserOutlines(query.PageIndex, query.PageSize, query.SearchText);
            return Ok(result);
        }

        /// <summary>
        /// 取得用户详细数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("userDetail")]
        public async Task<IActionResult> GetUserDetail([FromQuery]UserDetailQuery query)
        {
            var user = await _userService.GetUserDetail(query.UserId);
            return Ok(user);
        }

        /// <summary>
        /// 更新用户数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPut("userDetail")]
        public async Task<IActionResult> UpdateUserDetail([FromBody]UserDetail query)
        {
            await _userService.UpdateUserDetail(query);
            return Ok();
        }

        /// <summary>
        /// 删除用户数据
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpDelete("userDetail")]
        public async Task<IActionResult> DeleteUser([FromQuery]UserDetailQuery query)
        {
            await _userService.DeleteUser(query.UserId);
            return Ok();
        }

    }
}