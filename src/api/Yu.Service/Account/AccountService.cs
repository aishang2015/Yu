using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
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
        /// 手机重置用户密码
        /// </summary>
        /// <param name="phoneNumber">电话号码</param>
        /// <param name="newPassword">新密码</param>
        public async Task<bool> ResetUserPasswordByPhone(string phoneNumber)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return false;
            }

            //var token  = await _userManager.GeneratePasswordResetTokenAsync(user);
            // todo 生成token
            // todo 通过短信运营商SDK发送token

            return true;
        }

        /// <summary>
        /// 手机重置用户密码
        /// </summary>
        /// <param name="phoneNumber">电话号码</param>
        /// <param name="newPassword">新密码</param>
        public async Task<bool> ResetUserPasswordByPhone(string phoneNumber, string newPassword, string token)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.PhoneNumber == phoneNumber);
            if (user == null)
            {
                return false;
            }
			
            // todo 验证token
			// todo 修改密码
            //var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
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

            return result.Succeeded;
        }
    }
}
