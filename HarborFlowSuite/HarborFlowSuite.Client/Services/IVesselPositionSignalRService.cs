using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService : IAsyncDisposable
    {
        HubConnectionState ConnectionState { get; }
        event Action<HubConnectionState> OnConnectionStateChanged;
        event Action<string, double, double, double, double, string, string, VesselMetadataDto> OnPositionUpdateReceived;
        event Action<int> OnTotalVesselCountChanged;
        int TotalVesselCount { get; }
        Task StartConnection();
        Task StopConnection();
    }
}