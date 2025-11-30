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
        private readonly IVesselService _vesselService;
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, (VesselPositionUpdateDto Position, VesselMetadataDto Metadata)> _vessels = new();

        public event Action<VesselPositionUpdateDto> OnPositionUpdateReceived;
        public event Action<string, VesselMetadataDto> OnMetadataUpdateReceived;
        public event Action<int> OnTotalVesselCountChanged;

        public int TotalVesselCount => _vessels.Count;
        public IReadOnlyDictionary<string, (VesselPositionUpdateDto Position, VesselMetadataDto Metadata)> Vessels => _vessels;

        public event Action<HubConnectionState> OnConnectionStateChanged;

        public HubConnectionState ConnectionState => _hubConnection?.State ?? HubConnectionState.Disconnected;

        public VesselPositionSignalRService(HubConnection hubConnection, IVesselService vesselService)
        {
            _hubConnection = hubConnection;
            _vesselService = vesselService;
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
                _vessels.AddOrUpdate(update.MMSI,
                    (update, null),
                    (key, existing) => (update, existing.Metadata));

                OnTotalVesselCountChanged?.Invoke(_vessels.Count);
                OnPositionUpdateReceived?.Invoke(update);
            });

            _hubConnection.On<string, VesselMetadataDto>("ReceiveVesselMetadataUpdate", (mmsi, metadata) =>
            {
                if (_vessels.TryGetValue(mmsi, out var existing))
                {
                    // Merge metadata logic could go here, but for now simple replacement or merge if needed
                    // For simplicity, we'll just update the metadata part
                    var mergedMetadata = new VesselMetadataDto
                    {
                        Flag = !string.IsNullOrEmpty(metadata.Flag) ? metadata.Flag : existing.Metadata?.Flag ?? string.Empty,
                        Length = metadata.Length > 0 ? metadata.Length : existing.Metadata?.Length,
                        ImoNumber = !string.IsNullOrEmpty(metadata.ImoNumber) ? metadata.ImoNumber : existing.Metadata?.ImoNumber ?? string.Empty,
                        ShipName = !string.IsNullOrEmpty(metadata.ShipName) ? metadata.ShipName : existing.Metadata?.ShipName ?? string.Empty,
                        Callsign = !string.IsNullOrEmpty(metadata.Callsign) ? metadata.Callsign : existing.Metadata?.Callsign ?? string.Empty,
                        Geartype = !string.IsNullOrEmpty(metadata.Geartype) ? metadata.Geartype : existing.Metadata?.Geartype ?? string.Empty,
                        VesselType = !string.IsNullOrEmpty(metadata.VesselType) ? metadata.VesselType : existing.Metadata?.VesselType ?? string.Empty
                    };

                    // Also update the position info's VesselType if it's available in metadata
                    var updatedPosition = existing.Position;
                    if (updatedPosition != null && !string.IsNullOrEmpty(metadata.VesselType))
                    {
                        updatedPosition.VesselType = metadata.VesselType;
                    }

                    _vessels[mmsi] = (updatedPosition, mergedMetadata);
                }
                else
                {
                    // If we receive metadata for a vessel we don't have a position for yet, we might want to store it?
                    // Or wait for position. Let's store it with null position for now if that happens, 
                    // though usually position comes first or we need position to show on map.
                    // Actually, without position, we can't show it on map. So maybe just ignore or store.
                    // Let's store it.
                    _vessels[mmsi] = (null, metadata);
                }

                OnMetadataUpdateReceived?.Invoke(mmsi, metadata);
            });

            if (_hubConnection.State == HubConnectionState.Disconnected)
            {
                await _hubConnection.StartAsync();
                OnConnectionStateChanged?.Invoke(_hubConnection.State);

                // Fetch initial state from server to ensure we have all active vessels
                try
                {
                    var activeVessels = await _vesselService.GetActiveVessels();
                    foreach (var vessel in activeVessels)
                    {
                        _vessels.AddOrUpdate(vessel.MMSI,
                            (vessel, null),
                            (key, existing) => (vessel, existing.Metadata));
                    }
                    OnTotalVesselCountChanged?.Invoke(_vessels.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching active vessels: {ex.Message}");
                }
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