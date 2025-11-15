using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HarborFlowSuite.Client.Services;

namespace HarborFlowSuite.Client.Providers;

public class CustomAuthorizationMessageHandler : DelegatingHandler
{
    private readonly TokenService _tokenService;

    public CustomAuthorizationMessageHandler(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_tokenService.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.Token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
