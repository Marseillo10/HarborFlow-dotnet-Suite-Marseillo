using Microsoft.AspNetCore.SignalR.Client;
using HarborFlowSuite.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class VesselPositionSignalRService : IVesselPositionSignalRService, IAsyncDisposable
    {
        private HubConnection _hubConnection;

        public event Action<VesselPositionDto> OnPositionUpdateReceived;

        public VesselPositionSignalRService()
        {
            // HubConnection will be initialized in StartConnection
        }

        public async Task StartConnection(string hubUrl)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<VesselPositionDto>("ReceivePositionUpdate", (position) =>
            {
                OnPositionUpdateReceived?.Invoke(position);
            });

            await _hubConnection.StartAsync();
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