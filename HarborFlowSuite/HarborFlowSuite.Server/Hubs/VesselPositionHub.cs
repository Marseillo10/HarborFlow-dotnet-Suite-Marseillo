using Microsoft.AspNetCore.SignalR;
using HarborFlowSuite.Core.DTOs; // Assuming DTOs are in Core project

namespace HarborFlowSuite.Server.Hubs
{
    public class VesselPositionHub : Hub
    {
        public async Task SendPositionUpdate(VesselPositionDto position)
        {
            await Clients.All.SendAsync("ReceivePositionUpdate", position);
        }
    }
}