using Microsoft.AspNetCore.SignalR.Client;
using HarborFlowSuite.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HarborFlowSuite.Client.Services
{
    public class VesselPositionSignalRService : IVesselPositionSignalRService, IAsyncDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly ILogger<VesselPositionSignalRService> _logger;
        private readonly HashSet<string> _activeVessels = new HashSet<string>();

        public HubConnectionState ConnectionState => _hubConnection.State;
        public event Action<HubConnectionState>? OnConnectionStateChanged;

        public event Action<string, double, double, double, double, string, string, VesselMetadataDto>? OnPositionUpdateReceived;
        public event Action<int>? OnTotalVesselCountChanged;

        public int TotalVesselCount => _activeVessels.Count;

        public VesselPositionSignalRService(HubConnection hubConnection, ILogger<VesselPositionSignalRService> logger)
        {
            _hubConnection = hubConnection;
            _logger = logger;

            _hubConnection.Reconnecting += error =>
            {
                _logger.LogWarning("SignalR connection is reconnecting. Error: {Error}", error?.Message);
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                _logger.LogInformation("SignalR connection reconnected with ID: {ConnectionId}", connectionId);
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                _logger.LogError("SignalR connection closed. Error: {Error}", error?.Message);
                OnConnectionStateChanged?.Invoke(_hubConnection.State);
                return Task.CompletedTask;
            };
        }

        public async Task StartConnection()
        {
            _hubConnection.On<string, double, double, double, double, string, string, VesselMetadataDto>("ReceiveVesselPositionUpdate", (mmsi, lat, lon, heading, speed, name, vesselType, metadata) =>
            {
                if (_activeVessels.Add(mmsi))
                {
                    OnTotalVesselCountChanged?.Invoke(_activeVessels.Count);
                }
                OnPositionUpdateReceived?.Invoke(mmsi, lat, lon, heading, speed, name, vesselType, metadata);
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                try
                {
                    await _hubConnection.StartAsync();
                    _logger.LogInformation("SignalR connection started.");
                    OnConnectionStateChanged?.Invoke(_hubConnection.State);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error starting SignalR connection: {Error}", ex.Message);
                }
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
                _hubConnection.Reconnecting -= OnReconnecting;
                _hubConnection.Reconnected -= OnReconnected;
                _hubConnection.Closed -= OnClosed;
                await _hubConnection.DisposeAsync();
            }
        }

        private Task OnReconnecting(Exception? error)
        {
            _logger.LogWarning("SignalR connection is reconnecting. Error: {Error}", error?.Message);
            OnConnectionStateChanged?.Invoke(_hubConnection.State);
            return Task.CompletedTask;
        }

        private Task OnReconnected(string? connectionId)
        {
            _logger.LogInformation("SignalR connection reconnected with ID: {ConnectionId}", connectionId);
            OnConnectionStateChanged?.Invoke(_hubConnection.State);
            return Task.CompletedTask;
        }

        private Task OnClosed(Exception? error)
        {
            _logger.LogError("SignalR connection closed. Error: {Error}", error?.Message);
            OnConnectionStateChanged?.Invoke(_hubConnection.State);
            return Task.CompletedTask;
        }
    }
}