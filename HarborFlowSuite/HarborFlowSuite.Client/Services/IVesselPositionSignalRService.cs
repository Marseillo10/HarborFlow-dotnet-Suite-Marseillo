using HarborFlowSuite.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService
    {
        event Action<VesselPositionUpdateDto> OnPositionUpdateReceived;
        event Action<string, VesselMetadataDto> OnMetadataUpdateReceived;
        event Action<int> OnTotalVesselCountChanged;
        event Action<HubConnectionState> OnConnectionStateChanged;
        int TotalVesselCount { get; }
        HubConnectionState ConnectionState { get; }
        IReadOnlyDictionary<string, (VesselPositionUpdateDto Position, VesselMetadataDto Metadata)> Vessels { get; }
        Task StartConnection();
        Task StopConnection();
    }
}