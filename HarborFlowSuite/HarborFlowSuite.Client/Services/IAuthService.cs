using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services;

public interface IAuthService
{
    Task<string> GetCurrentUserToken();
    Task SignIn(string email, string password);
    Task SignOut();
}
