using HarborFlowSuite.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Server.Tests
{
    public class AisHubTests
    {
        [Fact]
        public async Task OnConnectedAsync_DoesNotThrowException()
        {
            // Arrange
            var hub = new AisHub();

            // Act & Assert
            await Record.ExceptionAsync(() => hub.OnConnectedAsync());
        }

        [Fact]
        public async Task OnDisconnectedAsync_DoesNotThrowException()
        {
            // Arrange
            var hub = new AisHub();
            var exception = new System.Exception("Test exception");

            // Act & Assert
            await Record.ExceptionAsync(() => hub.OnDisconnectedAsync(exception));
        }
    }
}
