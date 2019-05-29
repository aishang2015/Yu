using System.Threading.Tasks;

namespace Yu.Service.Account
{
    public interface IAccountService
    {
        Task<string> FindUser(string userName, string password);
    }
}
