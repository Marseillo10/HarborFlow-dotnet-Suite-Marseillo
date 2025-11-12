using Bunit;
using HarborFlowSuite.Client.Components;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HarborFlowSuite.Client.Tests
{
    public class MapComponentTests : TestContext
    {
        [Fact]
        public void MapComponent_RendersAndInitializesMap()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var vesselServiceMock = new Mock<IVesselService>();

            // Setup the mock for the specific JS call
            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("initMap", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());

            // Setup the mock for GetVesselPositions to return an empty list
            vesselServiceMock
                .Setup(s => s.GetVesselPositions())
                .ReturnsAsync(new List<VesselPositionDto>());

            // Register services
            Services.AddSingleton(jsRuntimeMock.Object);
            Services.AddSingleton(vesselServiceMock.Object);

            // Act
            var cut = Render<Map>();

            // Assert
            // Verify that the map div exists
            cut.Find("#map");

            // Verify that initMap was called once
            jsRuntimeMock.Verify(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()), Times.Once);
        }

        [Fact]
        public void MapComponent_RendersVesselIcons()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var vesselServiceMock = new Mock<IVesselService>();

            var testVesselPositions = new List<VesselPositionDto>
            {
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "TestVessel1", VesselType = "Cargo", Latitude = 1.0m, Longitude = 1.0m, Heading = 90, Speed = 10, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "TestVessel2", VesselType = "Tanker", Latitude = 2.0m, Longitude = 2.0m, Heading = 180, Speed = 15, RecordedAt = DateTime.UtcNow }
            };

            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());

            vesselServiceMock
                .Setup(s => s.GetVesselPositions())
                .ReturnsAsync(testVesselPositions);

            Services.AddSingleton(jsRuntimeMock.Object);
            Services.AddSingleton(vesselServiceMock.Object);

            // Act
            var cut = Render<Map>();

            // Assert
            jsRuntimeMock.Verify(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.addVesselMarkers", It.Is<object[]>(args =>
                args != null && args.Length == 1 && args[0] == testVesselPositions
            )), Times.Once);
        }

        [Fact]
        public async Task MapComponent_InteractsWithDataServiceAndRendersMarkers()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var vesselServiceMock = new Mock<IVesselService>();

            var expectedVesselPositions = new List<VesselPositionDto>
            {
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "ServiceVessel1", VesselType = "Cargo", Latitude = 1.1m, Longitude = 1.1m, Heading = 45, Speed = 10, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "ServiceVessel2", VesselType = "Tanker", Latitude = 2.2m, Longitude = 2.2m, Heading = 270, Speed = 12, RecordedAt = DateTime.UtcNow }
            };

            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());
            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.addVesselMarkers", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());

            vesselServiceMock
                .Setup(s => s.GetVesselPositions())
                .ReturnsAsync(expectedVesselPositions);

            Services.AddSingleton(jsRuntimeMock.Object);
            Services.AddSingleton(vesselServiceMock.Object);

            // Act
            var cut = Render<Map>();

            // Assert
            // Verify that GetVesselPositions was called on the service
            vesselServiceMock.Verify(s => s.GetVesselPositions(), Times.Once);

            // Verify that addVesselMarkers was called with the data from the service
            jsRuntimeMock.Verify(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.addVesselMarkers", It.Is<object[]>(args =>
                args != null && args.Length == 1 && args[0] == expectedVesselPositions
            )), Times.Once);
        }
    }
}
