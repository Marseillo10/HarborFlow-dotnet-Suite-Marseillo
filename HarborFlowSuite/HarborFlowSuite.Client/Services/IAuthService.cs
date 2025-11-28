using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services;

public interface IAuthService
{
    Task<string> GetCurrentUserToken();
    Task<bool> SignIn(string email, string password);
    Task SignOut();
    Task SendPasswordResetEmail(string email);
}
