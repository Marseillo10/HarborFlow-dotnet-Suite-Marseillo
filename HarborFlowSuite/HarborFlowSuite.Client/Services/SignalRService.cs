using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class SignalRService : ISignalRService, IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;

        public event Action<string, double, double, double, double, string> OnVesselPositionUpdate;

        public SignalRService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;

            _hubConnection.On<string, double, double, double, double, string>("ReceiveVesselPositionUpdate",
                (mmsi, lat, lon, heading, speed, name) =>
                {
                    OnVesselPositionUpdate?.Invoke(mmsi, lat, lon, heading, speed, name);
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
