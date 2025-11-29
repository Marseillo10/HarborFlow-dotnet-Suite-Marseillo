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
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public FirebaseAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
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

    [JSInvokable]
    public void OnAuthStateChanged(FirebaseUserDto userDto)
    {
        Console.WriteLine("OnAuthStateChanged called");
        if (userDto == null)
        {
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            return;
        }

        var claims = ParseClaimsFromJwt(userDto.Token);

        // Add Name claim if DisplayName is available
        if (!string.IsNullOrEmpty(userDto.DisplayName))
        {
            claims.Add(new Claim(ClaimTypes.Name, userDto.DisplayName));
        }
        else if (!string.IsNullOrEmpty(userDto.Email))
        {
            // Fallback to email if no display name
            claims.Add(new Claim(ClaimTypes.Name, userDto.Email));
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    private static List<Claim> ParseClaimsFromJwt(string jwt)
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
            if (keyValuePairs.TryGetValue("name", out var name))
            {
                claims.Add(new Claim(ClaimTypes.Name, name.ToString() ?? string.Empty));
            }
            if (keyValuePairs.TryGetValue("role", out var role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString() ?? string.Empty));
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
    public string DisplayName { get; set; }
    public string Token { get; set; }
}

