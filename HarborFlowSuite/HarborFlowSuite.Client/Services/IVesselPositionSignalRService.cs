using HarborFlowSuite.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselPositionSignalRService
    {
        event Action<VesselPositionDto> OnPositionUpdateReceived;
        Task StartConnection(string hubUrl);
        Task StopConnection();
    }
}