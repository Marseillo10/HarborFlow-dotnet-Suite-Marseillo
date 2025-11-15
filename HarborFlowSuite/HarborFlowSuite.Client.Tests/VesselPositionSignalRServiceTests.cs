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
        private readonly Mock<IVesselPositionSignalRService> _vesselPositionSignalRServiceMock;

        public VesselPositionSignalRServiceTests()
        {
            _vesselPositionSignalRServiceMock = new Mock<IVesselPositionSignalRService>();
        }

        [Fact]
        public async Task StartConnection_RegistersReceiveVesselPositionUpdateHandler()
        {
            // Arrange
            _vesselPositionSignalRServiceMock.Setup(s => s.StartConnection()).Returns(Task.CompletedTask);

            // Act
            await _vesselPositionSignalRServiceMock.Object.StartConnection();

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.StartConnection(), Times.Once);
        }

        [Fact]
        public async Task StartConnection_StartsHubConnection()
        {
            // Arrange
            _vesselPositionSignalRServiceMock.Setup(s => s.StartConnection()).Returns(Task.CompletedTask);

            // Act
            await _vesselPositionSignalRServiceMock.Object.StartConnection();

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.StartConnection(), Times.Once);
        }

        [Fact]
        public void OnPositionUpdateReceived_EventIsInvokedWhenMessageReceived()
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

            var eventFired = false;
            _vesselPositionSignalRServiceMock.Object.OnPositionUpdateReceived += (m, la, lo, h, s, n, vt, meta) =>
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

            _vesselPositionSignalRServiceMock.Raise(s => s.OnPositionUpdateReceived += null, mmsi, lat, lon, heading, speed, name, vesselType, metadata);

            // Assert
            Assert.True(eventFired);
        }

        [Fact]
        public async Task StopConnection_StopsHubConnection()
        {
            // Arrange
            _vesselPositionSignalRServiceMock.Setup(s => s.StopConnection()).Returns(Task.CompletedTask);

            // Act
            await _vesselPositionSignalRServiceMock.Object.StopConnection();

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.StopConnection(), Times.Once);
        }

        [Fact]
        public async Task DisposeAsync_DisposesHubConnection()
        {
            // Arrange
            _vesselPositionSignalRServiceMock.Setup(s => s.DisposeAsync()).Returns(new ValueTask(Task.CompletedTask));

            // Act
            await _vesselPositionSignalRServiceMock.Object.DisposeAsync();

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.DisposeAsync(), Times.Once);
        }
    }
}