using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Yu.Core.Mvc;
using Yu.Model.WebAdmin.User;
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
        public IActionResult GetUserOutlines([FromQuery]int pageIndex, [FromQuery]int pageSize, [FromQuery]string searchText)
        {
            // 校验参数
            if (pageIndex <= 0 || pageSize <= 0 || pageSize > 100)
            {
                ModelState.AddModelError("PageIndex,PageSize", ErrorMessages.WebAdmin_User_E001);
                return BadRequest(ModelState);
            }

            // 取得数据            
            var result = _userService.GetUserOutlines(pageIndex, pageSize, searchText);
            return Ok(result);
        }

    }
}