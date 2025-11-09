using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace HarborFlowSuite.Client.Services;

public class AuthService : IAuthService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> GetCurrentUserToken()
    {
        return await _jsRuntime.InvokeAsync<string>("firebaseAuth.getCurrentUserToken");
    }

    public async Task SignIn(string email, string password)
    {
        await _jsRuntime.InvokeVoidAsync("firebaseAuth.signIn", email, password);
    }

    public async Task SignOut()
    {
        await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
    }
}
