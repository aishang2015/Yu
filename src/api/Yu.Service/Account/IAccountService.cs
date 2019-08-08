using System.Threading.Tasks;
using Yu.Data.Infrasturctures;

namespace Yu.Service.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        Task<BaseIdentityUser> FindUserAsync(string userName);

        /// <summary>
        /// 确认用户名密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<BaseIdentityUser> FindUserAsync(string userName, string password);

        /// <summary>
        /// 根据旧密码修改新密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        Task<bool> ChangePasswordAsync(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// 账号绑定手机，发送验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="phoneNumber">电话号码</param>
        /// <remarks>如果是使用账号直接注册的情况下，先用手机号生成一个用户</remarks>
        Task<bool> VerifyPhoneNumberAsync(string userName, string phoneNumber);

        /// <summary>
        /// 账号绑定手机，确认验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task<bool> VerifyPhoneNumberAsync(string userName, string phoneNumber, string code);

        /// <summary>
        /// 查找用户角色
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        Task<string> FindUserRoleAsync(BaseIdentityUser user);

        /// <summary>
        /// 手机重置用户密码
        /// </summary>
        /// <param name="phoneNumber">电话号码</param>
        /// <param name="newPassword">新密码</param>
        Task<bool> ResetUserPasswordByPhone(string phoneNumber);

        /// <summary>
        /// 手机重置用户密码
        /// </summary>
        /// <param name="phoneNumber">电话号码</param>
        /// <param name="newPassword">新密码</param>
        Task<bool> ResetUserPasswordByPhone(string phoneNumber, string newPassword, string code);
    }
}
