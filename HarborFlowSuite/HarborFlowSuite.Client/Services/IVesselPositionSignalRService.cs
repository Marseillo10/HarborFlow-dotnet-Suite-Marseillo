using HarborFlowSuite.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService
    {
        event Action<string, double, double, double, double, string, string, VesselMetadataDto> OnPositionUpdateReceived;
        Task StartConnection();
        Task StopConnection();
    }
}