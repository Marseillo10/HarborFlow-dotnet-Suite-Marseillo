using HarborFlowSuite.Abstractions.Services;
using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace HarborFlowSuite.Client.Services;

public class AuthService : IClientAuthService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetCurrentUserToken()
    {
        return await _jsRuntime.InvokeAsync<string>("firebaseAuth.getCurrentUserToken");
    }

    public async Task<string?> SignIn(string email, string password)
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseAuth.signIn", email, password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Firebase sign-in JS interop: {ex.Message}");
            return null;
        }
    }

    public async Task SignOut()
    {
        await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
    }
}