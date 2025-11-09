using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using HarborFlowSuite.Client.Providers;

namespace HarborFlowSuite.Client.Services;

public class AuthService : IAuthService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly FirebaseAuthenticationStateProvider _authenticationStateProvider;

    public AuthService(IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider)
    {
        _jsRuntime = jsRuntime;
        _authenticationStateProvider = (FirebaseAuthenticationStateProvider)authenticationStateProvider;
    }

    public async Task<string> GetCurrentUserToken()
    {
        return await _jsRuntime.InvokeAsync<string>("firebaseAuth.getCurrentUserToken");
    }

    public async Task<bool> SignIn(string email, string password)
    {
        try
        {
            bool success = await _jsRuntime.InvokeAsync<bool>("firebaseAuth.signIn", email, password);
            if (success)
            {
                var token = await GetCurrentUserToken();
                _authenticationStateProvider.AuthenticateUser(token);
            }
            return success;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Firebase sign-in JS interop: {ex.Message}");
            return false;
        }
    }

    public async Task SignOut()
    {
        await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
        _authenticationStateProvider.AuthenticateUser(null);
    }
}
