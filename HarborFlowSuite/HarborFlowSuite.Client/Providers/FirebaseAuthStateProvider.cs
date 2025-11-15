using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using HarborFlowSuite.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace HarborFlowSuite.Client.Providers
{
    public class FirebaseAuthStateProvider : AuthenticationStateProvider, IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly TokenService _tokenService;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private DotNetObjectReference<FirebaseAuthStateProvider> _dotNetObjectReference;

        public FirebaseAuthStateProvider(IJSRuntime jsRuntime, TokenService tokenService)
        {
            _jsRuntime = jsRuntime;
            _tokenService = tokenService;
            _dotNetObjectReference = DotNetObjectReference.Create(this);
            _jsRuntime.InvokeVoidAsync("firebaseAuth.onTokenChanged", _dotNetObjectReference);
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("firebaseAuth.getCurrentUserToken");
            if (!string.IsNullOrEmpty(token))
            {
                _tokenService.Token = token;
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            }
            else
            {
                _tokenService.Token = null;
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            return new AuthenticationState(_currentUser);
        }

        [JSInvokable]
        public void OnTokenChanged(string token)
        {
            _tokenService.Token = token;
            if (!string.IsNullOrEmpty(token))
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            }
            else
            {
                _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs.TryGetValue(ClaimTypes.Name, out var name))
            {
                claims.Add(new Claim(ClaimTypes.Name, name.ToString()));
            }
            if (keyValuePairs.TryGetValue(ClaimTypes.Email, out var email))
            {
                claims.Add(new Claim(ClaimTypes.Email, email.ToString()));
            }
            if (keyValuePairs.TryGetValue("user_id", out var userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
            }

            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        public void Dispose()
        {
            _dotNetObjectReference?.Dispose();
        }
    }
}
