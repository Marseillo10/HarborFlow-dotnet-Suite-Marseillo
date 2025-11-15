using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class MediaQuery
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<MediaQuery>? _dotNetObjectReference;
        private IJSObjectReference? _jsModule;

        public static bool IsDesktop { get; private set; }
        public event Action? OnMediaQueryChanged;

        public MediaQuery(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        public async Task Initialize()
        {
            _jsModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/media-query.js");
            await _jsModule.InvokeVoidAsync("init", _dotNetObjectReference);
        }

        [JSInvokable]
        public void SetIsDesktop(bool isDesktop)
        {
            IsDesktop = isDesktop;
            OnMediaQueryChanged?.Invoke();
        }

        public async ValueTask CleanupAsync()
        {
            if (_jsModule != null)
            {
                await _jsModule.DisposeAsync();
            }
            _dotNetObjectReference?.Dispose();
        }
    }
}
