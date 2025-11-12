using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface ISignalRService
    {
        event Action<string, double, double, double, double, string> OnVesselPositionUpdate;
        Task StartConnection();
        Task StopConnection();
    }
}
