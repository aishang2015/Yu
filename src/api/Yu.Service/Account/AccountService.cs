using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Yu.Data.Infrasturctures;

namespace Yu.Service.Account
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<BaseIdentityUser> _userManager;

        private readonly RoleManager<BaseIdentityRole> _roleManager;

        public AccountService(UserManager<BaseIdentityUser> userManager, RoleManager<BaseIdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 验证用户密码
        public async Task<string> FindUser(string userName, string password)
        {
            // 查找用户
            var user = await _userManager.FindByNameAsync(userName);

            // 用户存在
            if (user != null) { 

                // 验证密码
                var result = await _userManager.CheckPasswordAsync(user, password);

                // 密码正确
                if (result)
                {
                    return user.UserName;
                }
            }
            return string.Empty;

        }
    }
}
