using Microsoft.AspNetCore.SignalR;

namespace HarborFlowSuite.Server.Hubs
{
    public class ServiceRequestHub : Hub
    {
        public async Task SendRequestUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveRequestUpdate", message);
        }
    }
}
