using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HarborFlowSuite.Client.Services;

namespace HarborFlowSuite.Client.Providers;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly IAuthService _authService;

    public CustomAuthorizationMessageHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _authService.GetCurrentUserToken();
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"Attaching token: {token}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            Console.WriteLine("No token found.");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
