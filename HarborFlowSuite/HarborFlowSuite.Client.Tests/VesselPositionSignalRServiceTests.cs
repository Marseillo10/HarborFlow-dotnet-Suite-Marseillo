using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Client.Tests
{
    public class VesselPositionSignalRServiceTests
    {
        private readonly Mock<HubConnection> _mockHubConnection;
        private readonly VesselPositionSignalRService _service;

        public VesselPositionSignalRServiceTests()
        {
            var mockHubConnectionBuilder = new Mock<IHubConnectionBuilder>();
            _mockHubConnection = new Mock<HubConnection>(mockHubConnectionBuilder.Object);
            _service = new VesselPositionSignalRService(_mockHubConnection.Object);
        }

        [Fact]
        public async Task StartConnection_RegistersReceiveVesselPositionUpdateHandler()
        {
            // Arrange
            _mockHubConnection.Setup(hc => hc.StartAsync(System.Threading.CancellationToken.None)).Returns(Task.CompletedTask);

            // Act
            await _service.StartConnection();

            // Assert
            _mockHubConnection.Verify(
                hc => hc.On(
                    "ReceiveVesselPositionUpdate",
                    It.IsAny<Action<string, double, double, double, double, string, string, VesselMetadataDto>>()),
                Times.Once);
        }

        [Fact]
        public async Task StartConnection_StartsHubConnection()
        {
            // Arrange
            _mockHubConnection.Setup(hc => hc.StartAsync(System.Threading.CancellationToken.None)).Returns(Task.CompletedTask);
            _mockHubConnection.SetupGet(hc => hc.State).Returns(HubConnectionState.Disconnected);

            // Act
            await _service.StartConnection();

            // Assert
            _mockHubConnection.Verify(hc => hc.StartAsync(System.Threading.CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task OnPositionUpdateReceived_EventIsInvokedWhenMessageReceived()
        {
            // Arrange
            string mmsi = "123456789";
            double lat = 10.0;
            double lon = 20.0;
            double heading = 90.0;
            double speed = 15.0;
            string name = "Test Vessel";
            string vesselType = "Cargo";
            VesselMetadataDto metadata = new VesselMetadataDto { Flag = "USA", ImoNumber = "IMO123", Length = 100.0 };

            Action<string, double, double, double, double, string, string, VesselMetadataDto> callback = null;
            _mockHubConnection.Setup(hc => hc.On(
                "ReceiveVesselPositionUpdate",
                It.IsAny<Action<string, double, double, double, double, string, string, VesselMetadataDto>>()))
                .Callback<string, Action<string, double, double, double, double, string, string, VesselMetadataDto>>((eventName, action) =>
                {
                    callback = action;
                });

            await _service.StartConnection();

            var eventFired = false;
            _service.OnPositionUpdateReceived += (m, la, lo, h, s, n, vt, meta) =>
            {
                eventFired = true;
                Assert.Equal(mmsi, m);
                Assert.Equal(lat, la);
                Assert.Equal(lon, lo);
                Assert.Equal(heading, h);
                Assert.Equal(speed, s);
                Assert.Equal(name, n);
                Assert.Equal(vesselType, vt);
                Assert.Equal(metadata, meta);
            };

            // Act
            callback?.Invoke(mmsi, lat, lon, heading, speed, name, vesselType, metadata);

            // Assert
            Assert.True(eventFired);
        }

        [Fact]
        public async Task StopConnection_StopsHubConnection()
        {
            // Arrange
            _mockHubConnection.Setup(hc => hc.StopAsync(System.Threading.CancellationToken.None)).Returns(Task.CompletedTask);
            _mockHubConnection.SetupGet(hc => hc.State).Returns(HubConnectionState.Connected);

            // Act
            await _service.StopConnection();

            // Assert
            _mockHubConnection.Verify(hc => hc.StopAsync(System.Threading.CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_DisposesHubConnection()
        {
            // Arrange
            _mockHubConnection.Setup(hc => hc.DisposeAsync()).Returns(new ValueTask(Task.CompletedTask));

            // Act
            await _service.DisposeAsync();

            // Assert
            _mockHubConnection.Verify(hc => hc.DisposeAsync(), Times.Once);
        }
    }
}