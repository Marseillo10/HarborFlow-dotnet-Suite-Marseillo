using Bunit;
using HarborFlowSuite.Client.Components;
using HarborFlowSuite.Client.Pages;
using HarborFlowSuite.Client.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.JSInterop.Infrastructure;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace HarborFlowSuite.Client.Tests
{
    public class MapSearchTests : BunitContext
    {
        private readonly Mock<IJSRuntime> _jsRuntimeMock;
        private readonly Mock<IPortService> _portServiceMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<SidebarService> _sidebarServiceMock;
        private readonly Mock<IVesselPositionSignalRService> _vesselPositionSignalRServiceMock;


        public MapSearchTests()
        {
            _jsRuntimeMock = new Mock<IJSRuntime>();
            _portServiceMock = new Mock<IPortService>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _sidebarServiceMock = new Mock<SidebarService>();
            _vesselPositionSignalRServiceMock = new Mock<IVesselPositionSignalRService>();


            Services.AddSingleton(_jsRuntimeMock.Object);
            Services.AddSingleton(_portServiceMock.Object);
            Services.AddSingleton(_httpClientFactoryMock.Object);
            Services.AddSingleton(_sidebarServiceMock.Object);
            Services.AddSingleton<IVesselPositionSignalRService>(_vesselPositionSignalRServiceMock.Object);
            Services.AddMudServices();
        }

        [Fact]
        public async Task MapSearch_FiltersVessels_When_SearchTermIsEntered()
        {
            // Arrange
            var searchTerm = "Test Vessel";

            _jsRuntimeMock.Setup(js => js.InvokeAsync<IJSVoidResult>(It.IsAny<string>(), It.IsAny<object[]>()))
                          .Returns(new ValueTask<IJSVoidResult>(Mock.Of<IJSVoidResult>()));
            
            _portServiceMock.Setup(p => p.GetPortsAsync(It.IsAny<string[]>())).ReturnsAsync(new List<Port>());

            var mockHttpClient = new HttpClient(new Mock<HttpMessageHandler>().Object);
            _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);


            // Act
            var cut = Render<Dashboard>();
            var mapSearchInput = cut.FindComponent<MapSearchInput>();

            // Directly invoke the OnSearchTermChanged parameter of the mocked component
            await mapSearchInput.InvokeAsync(() => mapSearchInput.Instance.OnSearchTermChanged.InvokeAsync(searchTerm));
            
            // Assert
            _jsRuntimeMock.Verify(js => js.InvokeAsync<IJSVoidResult>("HarborFlowMap.filterVessels", It.Is<object[]>(o => (string)o[0] == searchTerm)), Times.Once);
        }
    }
}
