using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Jwt;
using Yu.Core.Mvc;
using Yu.Model.Account.InputModels;
using Yu.Model.Account.OutputModels;
using Yu.Model.Message;
using Yu.Service.Account;

namespace Yu.Api.Controllers
{
    [Description("账户管理")]
    public class AccountController : AnonymousController
    {
        private readonly IJwtFactory _jwtFactory;

        private readonly IAccountService _accountService;

        private readonly IMemoryCache _memoryCache;

        public AccountController(IJwtFactory jwtFactory, IAccountService accountService, IMemoryCache memoryCache)
        {
            _jwtFactory = jwtFactory;
            _accountService = accountService;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [Description("用户名密码登陆")]
        public async Task<IActionResult> Login([FromBody]LoginModel model)
        {

            // 针对app普通用户,商户用户,平台管理员等不同的用户角色考虑直接在userclaim上添加不同平台的区分
            // 例如后台运维用户则直接在用户声明里添加(userPlatformType,customerUser/bussinessUser/operatioinUser)

            // 检查验证码
            var result = _memoryCache.TryGetValue(model.CaptchaCodeId, out string code);

            // 无法取得说明已过期
            if (!result)
            {
                ModelState.AddModelError("CaptchaCode", ErrorMessages.Account_E004);
                return BadRequest(ModelState);
            }
            else
            {
                // 验证数值是否相同
                if (code != model.CaptchaCode)
                {
                    ModelState.AddModelError("CaptchaCode", ErrorMessages.Account_E005);
                    return BadRequest(ModelState);
                }
            }

            // 验证通过删除验证码缓存
            _memoryCache.Remove(model.CaptchaCodeId);

            // 检查用户名密码
            var user = await _accountService.FindUser(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("UserName,Password", ErrorMessages.Account_E006);
                return BadRequest(ModelState);
            }

            // 生成JwtToken
            var token = _jwtFactory.GenerateJwtToken(new List<(string, string)>{
                (CustomClaimTypes.UserName,user.UserName)
            });

            // 返回结果
            return Ok(new LoginResult
            {
                Token = token,
                UserName = user.UserName,
                AvatarUrl = user.Avatar ?? string.Empty
            });
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns>新token</returns>
        [HttpPost]
        [Description("刷新token")]
        public IActionResult RefreshToken()
        {
            var oldToken = Request.Headers["Authorization"].ToString();
            var newtoken = _jwtFactory.RefreshJwtToken(oldToken?.Replace("Bearer ", string.Empty));

            // token刷新失败
            if (string.IsNullOrEmpty(newtoken))
            {
                ModelState.AddModelError("Token", ErrorMessages.Account_E007);
                return BadRequest(ModelState);
            }
            return Ok(new { token = newtoken });
        }

    }
}