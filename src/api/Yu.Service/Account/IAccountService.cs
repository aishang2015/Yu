using System.Threading.Tasks;
using Yu.Data.Infrasturctures;

namespace Yu.Service.Account
{
    public interface IAccountService
    {
        Task<BaseIdentityUser> FindUser(string userName, string password);

        /// <summary>
        /// 根据旧密码修改新密码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassword">新密码</param>
        /// <returns></returns>
        Task<bool> ChangePassword(string userName, string oldPassword, string newPassword);

        /// <summary>
        /// 账号绑定手机，发送验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="phoneNumber">电话号码</param>
        /// <remarks>如果是使用账号直接注册的情况下，先用手机号生成一个用户</remarks>
        Task<bool> VerifyPhoneNumber(string userName, string phoneNumber);

        /// <summary>
        /// 账号绑定手机，确认验证码
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        Task<bool> VerifyPhoneNumber(string userName, string phoneNumber, string code);
    }
}
