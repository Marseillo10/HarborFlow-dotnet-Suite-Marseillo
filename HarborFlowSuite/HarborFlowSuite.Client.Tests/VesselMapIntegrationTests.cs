using Bunit;
using HarborFlowSuite.Client.Components;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Shared.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.JSInterop.Infrastructure;

namespace HarborFlowSuite.Client.Tests
{
    public class VesselMapIntegrationTests : TestContext
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IVesselPositionSignalRService> _vesselPositionSignalRServiceMock;

        public VesselMapIntegrationTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _vesselPositionSignalRServiceMock = new Mock<IVesselPositionSignalRService>();

            Services.AddSingleton(_jsRuntimeMock.Object);
            Services.AddSingleton(_vesselPositionSignalRServiceMock.Object);
        }

        [Fact]
        public async Task VesselMap_ReceivesRealtimeUpdatesAndCallsUpdateVesselMarker()
        {
            // Arrange
            var mmsi = "123456789";
            var lat = 34.05;
            var lng = -118.25;
            var heading = 90.0;
            var speed = 15.0;
            var name = "Test Vessel";
            var vesselType = "Cargo";
            var metadata = new VesselMetadataDto { Flag = "USA", ImoNumber = "IMO123", Length = 100.0 };

            // Setup JSInterop mock for initMap
            _jsRuntimeMock.Setup(js => js.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                          .Returns(new ValueTask<IJSVoidResult>(Mock.Of<IJSVoidResult>()));

            // Capture the event handler when the component subscribes
            // Capture the event handler when the component subscribes
            Action<VesselPositionUpdateDto> onPositionUpdateReceived = null;
            _vesselPositionSignalRServiceMock.SetupAdd(m => m.OnPositionUpdateReceived += It.IsAny<Action<VesselPositionUpdateDto>>())
                                             .Callback<Action<VesselPositionUpdateDto>>(handler => onPositionUpdateReceived = handler);

            // Act
            var cut = Render<VesselMap>();

            // Simulate a position update
            await cut.InvokeAsync(() => onPositionUpdateReceived?.Invoke(new VesselPositionUpdateDto
            {
                MMSI = mmsi,
                Latitude = lat,
                Longitude = lng,
                Heading = heading,
                Speed = speed,
                Name = name,
                VesselType = vesselType,
                Metadata = metadata
            }));

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.StartConnection(), Times.Once);
            _jsRuntimeMock.Verify(js => js.InvokeAsync<IJSVoidResult>("HarborFlowMap.updateVesselMarker", It.Is<object[]>(o =>
                (string)o[0] == mmsi &&
                (double)o[1] == lat &&
                (double)o[2] == lng &&
                (double)o[3] == heading &&
                (double)o[4] == speed &&
                (string)o[5] == name &&
                (string)o[6] == vesselType &&
                (VesselMetadataDto)o[7] == metadata
            )), Times.Once);
        }

        [Fact(Skip = "BUnit async disposal issue")]
        public async Task VesselMap_DisposesSignalRServiceOnDispose()
        {
            // Arrange
            _jsRuntimeMock.Setup(js => js.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                          .Returns(new ValueTask<IJSVoidResult>(Mock.Of<IJSVoidResult>()));

            // Act
            var cut = Render<VesselMap>();
            cut.Dispose();

            // Assert
            _vesselPositionSignalRServiceMock.Verify(s => s.StopConnection(), Times.Once);
            _vesselPositionSignalRServiceMock.VerifyRemove(s => s.OnPositionUpdateReceived -= It.IsAny<Action<VesselPositionUpdateDto>>(), Times.Once);
        }
    }
}
