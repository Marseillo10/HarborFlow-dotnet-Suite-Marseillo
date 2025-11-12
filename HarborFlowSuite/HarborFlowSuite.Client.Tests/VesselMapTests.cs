using Bunit;
using HarborFlowSuite.Client.Components;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.JSInterop.Infrastructure;

namespace HarborFlowSuite.Client.Tests
{
    public class VesselMapTests : TestContext
    {
        [Fact]
        public void VesselMap_ReceivesDtoPositionUpdate_CallsUpdateVesselMarker()
        {
            // Arrange
            var jsRuntimeMock = new Mock<IJSRuntime>();
            var signalRServiceMock = new Mock<ISignalRService>();
            var vesselPositionSignalRServiceMock = new Mock<IVesselPositionSignalRService>();

            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());

            jsRuntimeMock
                .Setup(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.updateVesselMarker", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>());

            Services.AddSingleton(jsRuntimeMock.Object);
            Services.AddSingleton(signalRServiceMock.Object);
            Services.AddSingleton(vesselPositionSignalRServiceMock.Object);

            var cut = Render<VesselMap>();

            var positionUpdate = new VesselPositionDto
            {
                VesselId = Guid.NewGuid(),
                VesselName = "TestVessel",
                VesselType = "Cargo",
                Latitude = 10.5m,
                Longitude = 20.5m,
                Heading = 180,
                Speed = 15,
                RecordedAt = DateTime.UtcNow
            };

            // Act
            // Trigger the event on the mock service
            vesselPositionSignalRServiceMock.Raise(s => s.OnPositionUpdateReceived += null, positionUpdate);

            // Assert
            // Assert that UpdateVesselMarker was called with the correct DTO data
            jsRuntimeMock.Verify(r => r.InvokeAsync<IJSVoidResult>("HarborFlowMap.updateVesselMarker", It.Is<object[]>(args =>
                args != null &&
                args.Length == 7 &&
                (string)args[0] == positionUpdate.VesselId.ToString() &&
                (decimal)args[1] == positionUpdate.Latitude &&
                (decimal)args[2] == positionUpdate.Longitude &&
                (decimal)args[3] == positionUpdate.Heading &&
                (decimal)args[4] == positionUpdate.Speed &&
                (string)args[5] == positionUpdate.VesselName &&
                (string)args[6] == positionUpdate.VesselType
            )), Times.Once);
        }
    }
}
