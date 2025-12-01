using Bunit;
using HarborFlowSuite.Client.Components;
using HarborFlowSuite.Core.DTOs;
using Xunit;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using HarborFlowSuite.Client.Services;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.JSInterop.Infrastructure;
using System;
using MudBlazor;

namespace HarborFlowSuite.Client.Tests
{
    public class VesselTooltipTests : BunitContext
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IVesselPositionSignalRService> _vesselPositionSignalRServiceMock;

        public VesselTooltipTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _vesselPositionSignalRServiceMock = new Mock<IVesselPositionSignalRService>();

            Services.AddSingleton(_jsRuntimeMock.Object);
            Services.AddSingleton(_vesselPositionSignalRServiceMock.Object);
            Services.AddSingleton(new Mock<IDialogService>().Object);

            // Setup JSInterop mock for initMap, as VesselMap will call it
            _jsRuntimeMock.Setup(js => js.InvokeAsync<IJSVoidResult>("HarborFlowMap.initMap", It.IsAny<object[]>()))
                          .Returns(new ValueTask<IJSVoidResult>(Mock.Of<IJSVoidResult>()));
        }

        [Fact]
        public void VesselTooltip_DisplaysCorrectlyWhenVisible()
        {
            // Arrange
            var vessel = new VesselPositionDto
            {
                VesselId = "123456789", // Use string for VesselId
                VesselName = "Test Vessel",
                VesselType = "Cargo", // Added VesselType
                IMO = "IMO1234567",
                VesselStatus = "Underway",
                Latitude = 1.0m, // Changed to decimal
                Longitude = 1.0m, // Changed to decimal
                Heading = 90m, // Changed to decimal
                Speed = 10m, // Changed to decimal
                RecordedAt = DateTime.UtcNow // Added RecordedAt
            };
            var position = new Point(100, 200);

            // Act
            var cut = Render<VesselTooltip>(parameters => parameters
                .Add(p => p.IsVisible, true)
                .Add(p => p.Vessel, vessel)
                .Add(p => p.Position, position)
            );

            // Assert
            Assert.Contains("Test Vessel", cut.Markup);
            Assert.Contains("IMO: IMO1234567", cut.Markup);
            Assert.Contains("Status: Underway", cut.Markup);
            Assert.Contains($"left: {position.X}px", cut.Markup);
            Assert.Contains($"top: {position.Y}px", cut.Markup);
        }

        [Fact]
        public void VesselTooltip_HidesWhenNotVisible()
        {
            // Arrange
            var vessel = new VesselPositionDto
            {
                VesselId = "123456789", // Use string for VesselId
                VesselName = "Test Vessel",
                VesselType = "Cargo", // Added VesselType
                IMO = "IMO1234567",
                VesselStatus = "Underway",
                Latitude = 1.0m, // Changed to decimal
                Longitude = 1.0m, // Changed to decimal
                Heading = 90m, // Changed to decimal
                Speed = 10m, // Changed to decimal
                RecordedAt = DateTime.UtcNow // Added RecordedAt
            };
            var position = new Point(100, 200);

            // Act
            var cut = Render<VesselTooltip>(parameters => parameters
                .Add(p => p.IsVisible, false)
                .Add(p => p.Vessel, vessel)
                .Add(p => p.Position, position)
            );

            // Assert
            Assert.DoesNotContain("Test Vessel", cut.Markup);
            Assert.DoesNotContain("IMO: IMO1234567", cut.Markup);
            Assert.DoesNotContain("Status: Underway", cut.Markup);
        }

        /*
        [Fact]
        public async Task VesselMap_ShowVesselTooltip_UpdatesStateAndRendersTooltip()
        {
            // Arrange
            var vessel = new VesselPositionDto
            {
                VesselId = "123456789",
                VesselName = "Test Vessel",
                VesselType = "Cargo",
                IMO = "IMO1234567",
                VesselStatus = "Underway",
                Latitude = 1.0m,
                Longitude = 1.0m,
                Heading = 90m,
                Speed = 10m,
                RecordedAt = DateTime.UtcNow
            };
            var x = 100;
            var y = 200;

            var cut = Render<VesselMap>();

            // Act
            await cut.InvokeAsync(() => cut.Instance.ShowVesselTooltip(vessel, x, y));
            cut.Render(); // Re-render the component to reflect state changes

            // Assert
            var tooltipComponent = cut.FindComponent<VesselTooltip>();
            Assert.True(tooltipComponent.Instance.IsVisible);
            Assert.Equal(vessel, tooltipComponent.Instance.Vessel);
            Assert.Equal(new Point(x, y), tooltipComponent.Instance.Position);
            Assert.Contains("Test Vessel", tooltipComponent.Markup);
        }

        [Fact]
        public async Task VesselMap_HideVesselTooltip_UpdatesStateAndHidesTooltip()
        {
            // Arrange
            var vessel = new VesselPositionDto
            {
                VesselId = "123456789",
                VesselName = "Test Vessel",
                VesselType = "Cargo",
                IMO = "IMO1234567",
                VesselStatus = "Underway",
                Latitude = 1.0m,
                Longitude = 1.0m,
                Heading = 90m,
                Speed = 10m,
                RecordedAt = DateTime.UtcNow
            };
            var x = 100;
            var y = 200;

            var cut = Render<VesselMap>();
            await cut.InvokeAsync(() => cut.Instance.ShowVesselTooltip(vessel, x, y)); // Show tooltip first
            cut.Render();

            // Act
            await cut.InvokeAsync(() => cut.Instance.HideVesselTooltip());
            cut.Render(); // Re-render the component to reflect state changes

            // Assert
            var tooltipComponent = cut.FindComponent<VesselTooltip>();
            Assert.False(tooltipComponent.Instance.IsVisible);
            Assert.Null(tooltipComponent.Instance.Vessel);
            Assert.DoesNotContain("Test Vessel", tooltipComponent.Markup);
        }
        */
    }
}