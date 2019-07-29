using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yu.Core.Constants;
using Yu.Data.Entities.Right;
using Yu.Data.Infrasturctures;
using Yu.Data.Repositories;

namespace Yu.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<BaseIdentityUser> _userManager;

        private readonly RoleManager<BaseIdentityRole> _roleManager;

        private readonly IRepository<Group, Guid> _groupRepository;

        public AccountService(UserManager<BaseIdentityUser> userManager,
            RoleManager<BaseIdentityRole> roleManager,
            IRepository<Group, Guid> groupRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _groupRepository = groupRepository;
        }



        // 根据旧密码修改新密码
        public async Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result.Succeeded;
        }

        // 验证用户密码
        public async Task<BaseIdentityUser> FindUserAsync(string userName, string password)
        {
            // 查找用户
            var user = await _userManager.FindByNameAsync(userName);

            // 用户存在
            if (user != null)
            {

                // 验证密码
                var result = await _userManager.CheckPasswordAsync(user, password);

                // 密码正确
                if (result)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public Task<BaseIdentityUser> FindUserAsync(string userName)
        {
            // 查找用户
            var user = _userManager.FindByNameAsync(userName);

            // 用户存在
            if (user != null)
            {
                return user;

            }
            return null;
        }

        /// <summary>
        /// 查找用户的角色
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> FindUserRoleAsync(BaseIdentityUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return string.Join(',', roles);
        }

        /// <summary>
        /// 账号绑定手机，发送验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="phoneNumber">电话号码</param>
        /// <returns></returns>
        public async Task<bool> VerifyPhoneNumberAsync(string userName, string phoneNumber)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            var verfiyCode = _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumber);

            // todo 通过短信运营商SDK发送信息

            return true;
        }

        /// <summary>
        /// 账号绑定手机，确认验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="code">验证码</param>
        public async Task<bool> VerifyPhoneNumberAsync(string userName, string phoneNumber, string code)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return false;
            }

            // 验证验证码
            var result = await _userManager.ChangePhoneNumberAsync(user, code, phoneNumber);
            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
    }
}
