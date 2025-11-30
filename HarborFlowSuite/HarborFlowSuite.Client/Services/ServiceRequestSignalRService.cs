using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace HarborFlowSuite.Client.Services
{
    public interface IServiceRequestSignalRService
    {
        event Action OnRequestUpdated;
        Task StartConnection();
        Task StopConnection();
    }

    public class ServiceRequestSignalRService : IServiceRequestSignalRService, IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;

        public event Action OnRequestUpdated;

        public ServiceRequestSignalRService(NavigationManager navigationManager, IAuthService authService)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7274/serviceRequestHub", options =>
                {
                    options.AccessTokenProvider = () => authService.GetCurrentUserToken();
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<string>("ReceiveRequestUpdate", (message) =>
            {
                OnRequestUpdated?.Invoke();
            });
        }

        public async Task StartConnection()
        {
            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task StopConnection()
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.StopAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
