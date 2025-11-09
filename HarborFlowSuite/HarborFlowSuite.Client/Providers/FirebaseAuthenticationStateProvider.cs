using System.Security.Claims;
using System.Threading.Tasks;
using HarborFlowSuite.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace HarborFlowSuite.Client.Providers;

public class FirebaseAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly IJSRuntime _jsRuntime;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public FirebaseAuthenticationStateProvider(IAuthService authService, IJSRuntime jsRuntime)
    {
        _authService = authService;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authService.GetCurrentUserToken();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(_anonymous);
        }

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }

    public void AuthenticateUser(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            return;
        }

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    // This method is invoked from JavaScript when the Firebase auth state changes
    [JSInvokable]
    public void OnAuthStateChanged(FirebaseUserDto userDto)
    {
        AuthenticateUser(userDto?.Token);
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs != null)
        {
            if (keyValuePairs.TryGetValue("sub", out var sub))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.ToString() ?? string.Empty));
            }
            if (keyValuePairs.TryGetValue("email", out var email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email.ToString() ?? string.Empty));
            }
            // Add other claims as needed
        }

        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
}

public class FirebaseUserDto
{
    public string Email { get; set; }
    public string Uid { get; set; }
    public string Token { get; set; }
}

