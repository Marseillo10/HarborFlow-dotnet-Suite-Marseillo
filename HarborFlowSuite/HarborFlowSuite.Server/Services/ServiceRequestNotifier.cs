using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HarborFlowSuite.Server.Services
{
    public class ServiceRequestNotifier : IServiceRequestNotifier
    {
        private readonly IHubContext<ServiceRequestHub> _hubContext;

        public ServiceRequestNotifier(IHubContext<ServiceRequestHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyRequestUpdated()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveRequestUpdate", "Update");
        }
    }
}
