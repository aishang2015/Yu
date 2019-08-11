using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Core.Extensions;
using Yu.Core.Jwt;
using Yu.Core.Mvc;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.Pemission;
using Yu.Model.Account.InputModels;
using Yu.Model.Account.OutputModels;
using Yu.Model.Message;
using Yu.Service.Account;
using Yu.Service.WebAdmin.Role;
using Yu.Service.WebAdmin.Rule;
using Yu.Service.WebAdmin.User;

namespace Yu.Api.Controllers
{
    [Description("账户管理")]
    public class AccountController : AnonymousController
    {
        private readonly IJwtFactory _jwtFactory;

        private readonly IAccountService _accountService;

        private readonly IUserService _userService;

        private readonly IRoleService _roleService;

        private readonly IRuleService _ruleService;

        private readonly IPermissionCacheService _permissionCacheService;

        private readonly IMemoryCache _memoryCache;

        public AccountController(IJwtFactory jwtFactory,
            IAccountService accountService,
            IUserService userService,
            IRoleService roleService,
            IRuleService ruleService,
            IPermissionCacheService permissionCacheService,
            IMemoryCache memoryCache)
        {
            _jwtFactory = jwtFactory;
            _accountService = accountService;
            _userService = userService;
            _roleService = roleService;
            _ruleService = ruleService;
            _permissionCacheService = permissionCacheService;
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
            var user = await _accountService.FindUserAsync(model.UserName, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("UserName,Password", ErrorMessages.Account_E006);
                return BadRequest(ModelState);
            }

            var loginResult = await GetLoginResult(user);
            return Ok(loginResult);
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns>新token</returns>
        [HttpPost]
        [Description("刷新token")]
        public async Task<IActionResult> RefreshToken()
        {
            var oldToken = Request.Headers["Authorization"].ToString();

            // 解析旧Token
            var claimPrincipal = _jwtFactory.CanRefresh(oldToken?.Replace("Bearer ", string.Empty));

            // token刷新失败
            if (claimPrincipal == null)
            {
                ModelState.AddModelError("Token", ErrorMessages.Account_E007);
                return BadRequest(ModelState);
            }

            // 根据token保存的用户名取得用户,更新用户数据
            var userName = claimPrincipal.GetUserName();
            var user = await _accountService.FindUserAsync(userName);

            var loginResult = await GetLoginResult(user);
            return Ok(loginResult);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        [HttpGet]
        [Description("手机号重置密码")]
        public async Task<IActionResult> ResetPwdByPhone([FromQuery]string phoneNumber)
        {
            var userName = User.GetUserName();
            var result = await _accountService.ResetUserPasswordByPhone(phoneNumber);
            if (!result)
            {
                ModelState.AddModelError("Password", ErrorMessages.Account_E009);
                return BadRequest(ModelState);
            }
            return Ok();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        [HttpPost]
        [Description("手机号重置密码")]
        public async Task<IActionResult> ResetPwdByPhone([FromBody]PhoneChangePwdModel model)
        {
            var userName = User.GetUserName();
            var result = await _accountService.ResetUserPasswordByPhone(model.PhoneNumber, model.NewPassword, model.ValidCode);
            if (!result)
            {
                ModelState.AddModelError("Password", ErrorMessages.Account_E010);
                return BadRequest(ModelState);
            }
            return Ok();
        }
        /// <summary>
        /// 获取token，加载权限到缓存,生成response结果
        /// </summary>
        private async Task<LoginResult> GetLoginResult(BaseIdentityUser user)
        {
            // 根据用户的角色获取用户页面侧的权限内容
            List<string> identities = new List<string> { }, routes = new List<string> { };

            // 用户的角色
            var userRoles = await _userService.GetUserRolesAsync(user);

            foreach (var role in userRoles)
            {
                var identificationStr = await _permissionCacheService.GetRoleIdentificationAsync(role);
                identities.AddRange(identificationStr.Split(CommonConstants.StringConnectChar, StringSplitOptions.RemoveEmptyEntries));

                var routeStr = await _permissionCacheService.GetRoleRoutesAsync(role);
                routes.AddRange(routeStr.Split(CommonConstants.StringConnectChar, StringSplitOptions.RemoveEmptyEntries));
            }

            // 生成JwtToken
            var token = _jwtFactory.GenerateJwtToken(new List<(string, string)>{
                (CustomClaimTypes.UserName,user.UserName),
                (CustomClaimTypes.Role, string.Join(',',userRoles)),
            });

            // 保存jwtToken到缓存
            _jwtFactory.StoreToken(user.UserName, token);

            // 返回结果
            return new LoginResult
            {
                Token = token,
                UserName = user.UserName,
                AvatarUrl = user.Avatar ?? string.Empty,
                Identifycations = identities.Distinct().ToArray(),
                Routes = routes.Distinct().ToArray()
            };
        }




    }
}