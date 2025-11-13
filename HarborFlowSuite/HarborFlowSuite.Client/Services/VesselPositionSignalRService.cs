using Microsoft.AspNetCore.SignalR.Client;
using HarborFlowSuite.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class VesselPositionSignalRService : IVesselPositionSignalRService, IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;

        public event Action<string, double, double, double, double, string, string, VesselMetadataDto> OnPositionUpdateReceived;

        public VesselPositionSignalRService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
        }

        public async Task StartConnection()
        {
            _hubConnection.On<string, double, double, double, double, string, string, VesselMetadataDto>("ReceiveVesselPositionUpdate", (mmsi, lat, lon, heading, speed, name, vesselType, metadata) =>
            {
                OnPositionUpdateReceived?.Invoke(mmsi, lat, lon, heading, speed, name, vesselType, metadata);
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
            }
        }

        public async Task StopConnection()
        {
            if (_hubConnection is not null)
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