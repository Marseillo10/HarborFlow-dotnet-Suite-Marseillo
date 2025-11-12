using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Hubs
{
    public class AisHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            // Logic to handle new client connections can be added here.
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception exception)
        {
            // Logic to handle client disconnections can be added here.
            return base.OnDisconnectedAsync(exception);
        }
    }
}
