using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IIdleTimeoutService
    {
        Task InitializeAsync(int timeoutInMinutes);
        ValueTask DisposeAsync();
        event Action OnTimeout;
    }

    public class IdleTimeoutService : IIdleTimeoutService, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<IdleTimeoutService>? _dotNetHelper;

        public event Action? OnTimeout;

        public IdleTimeoutService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync(int timeoutInMinutes)
        {
            _dotNetHelper = DotNetObjectReference.Create(this);
            var timeoutInMs = timeoutInMinutes * 60 * 1000;
            await _jsRuntime.InvokeVoidAsync("idleTimer.initialize", _dotNetHelper, timeoutInMs);
        }

        [JSInvokable]
        public void Logout()
        {
            OnTimeout?.Invoke();
        }

        public async ValueTask DisposeAsync()
        {
            if (_dotNetHelper != null)
            {
                await _jsRuntime.InvokeVoidAsync("idleTimer.dispose");
                _dotNetHelper.Dispose();
            }
        }
    }
}
