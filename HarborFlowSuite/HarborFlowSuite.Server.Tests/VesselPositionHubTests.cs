using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Server.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Server.Tests
{
    public class VesselPositionHubTests
    {
        [Fact]
        public async Task SendPositionUpdate_BroadcastsToAllClients()
        {
            // Arrange
            var mockClients = new Mock<IHubCallerClients>();
            var mockClientProxy = new Mock<IClientProxy>();

            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

            var hub = new VesselPositionHub
            {
                Clients = mockClients.Object
            };

            var positionDto = new VesselPositionDto
            {
                VesselId = Guid.NewGuid(),
                VesselName = "Test Vessel",
                VesselType = "Cargo",
                Latitude = 34.0522m,
                Longitude = -118.2437m,
                Heading = 180,
                Speed = 15,
                RecordedAt = DateTime.UtcNow
            };

            // Act
            await hub.SendPositionUpdate(positionDto);

            // Assert
            mockClientProxy.Verify(
                proxy => proxy.SendCoreAsync(
                    "ReceivePositionUpdate",
                    It.Is<object[]>(o => ((VesselPositionDto)o[0]).VesselId == positionDto.VesselId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
