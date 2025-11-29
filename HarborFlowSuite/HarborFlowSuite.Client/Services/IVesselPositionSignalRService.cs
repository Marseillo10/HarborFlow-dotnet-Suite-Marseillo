using HarborFlowSuite.Shared.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService
    {
        event Action<VesselPositionUpdateDto> OnPositionUpdateReceived;
        event Action<string, VesselMetadataDto> OnMetadataUpdateReceived;
        event Action<int> OnTotalVesselCountChanged;
        event Action<Microsoft.AspNetCore.SignalR.Client.HubConnectionState> OnConnectionStateChanged;
        int TotalVesselCount { get; }
        Microsoft.AspNetCore.SignalR.Client.HubConnectionState ConnectionState { get; }
        Task StartConnection();
        Task StopConnection();
    }
}