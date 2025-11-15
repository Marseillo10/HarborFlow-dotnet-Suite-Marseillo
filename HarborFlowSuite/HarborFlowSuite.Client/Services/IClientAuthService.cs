
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IClientAuthService
    {
        Task<string> GetCurrentUserToken();
        Task<string> SignIn(string email, string password);
        Task SignOut();
    }
}
