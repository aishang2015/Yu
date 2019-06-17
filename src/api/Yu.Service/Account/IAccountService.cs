using System.Threading.Tasks;
using Yu.Data.Infrasturctures;

namespace Yu.Service.Account
{
    public interface IAccountService
    {
        Task<BaseIdentityUser> FindUser(string userName, string password);
    }
}
