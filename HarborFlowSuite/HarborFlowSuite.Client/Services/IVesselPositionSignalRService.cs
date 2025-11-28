using HarborFlowSuite.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService
    {
        event Action<string, double, double, double, double, string, string, VesselMetadataDto> OnPositionUpdateReceived;
        event Action<int> OnTotalVesselCountChanged;
        event Action<Microsoft.AspNetCore.SignalR.Client.HubConnectionState> OnConnectionStateChanged;
        int TotalVesselCount { get; }
        Microsoft.AspNetCore.SignalR.Client.HubConnectionState ConnectionState { get; }
        Task StartConnection();
        Task StopConnection();
    }
}