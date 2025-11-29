using Microsoft.AspNetCore.SignalR.Client;
using HarborFlowSuite.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class VesselPositionSignalRService : IVesselPositionSignalRService, IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly HashSet<string> _activeVessels = new HashSet<string>();

        public event Action<VesselPositionUpdateDto> OnPositionUpdateReceived;
        public event Action<string, VesselMetadataDto> OnMetadataUpdateReceived;
        public event Action<int> OnTotalVesselCountChanged;

        public int TotalVesselCount => _activeVessels.Count;

        public event Action<HubConnectionState> OnConnectionStateChanged;

        public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

        public VesselPositionSignalRService(HubConnection hubConnection)
        {
            _hubConnection = hubConnection;
            _hubConnection.Closed += (e) =>
            {
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };
            _hubConnection.Reconnecting += (e) =>
            {
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };
            _hubConnection.Reconnected += (s) =>
            {
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };
        }

        public async Task StartConnection()
        {
            _hubConnection.On<VesselPositionUpdateDto>("ReceiveVesselPositionUpdate", (update) =>
            {
                if (_activeVessels.Add(update.MMSI))
                {
                    OnTotalVesselCountChanged?.Invoke(_activeVessels.Count);
                }
                OnPositionUpdateReceived?.Invoke(update);
            });

            _hubConnection.On<string, VesselMetadataDto>("ReceiveVesselMetadataUpdate", (mmsi, metadata) =>
            {
                OnMetadataUpdateReceived?.Invoke(mmsi, metadata);
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
            }
        }

        public async Task StopConnection()
        {
            if (_hubConnection is not null)
            {
                await _hubConnection.StopAsync();
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
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