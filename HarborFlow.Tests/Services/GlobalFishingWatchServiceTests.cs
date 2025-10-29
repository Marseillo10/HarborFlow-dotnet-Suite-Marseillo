using HarborFlow.Application.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Tests.Services
{
    public class GlobalFishingWatchServiceTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<ILogger<GlobalFishingWatchService>> _mockLogger;
        private readonly GlobalFishingWatchService _service;

        public GlobalFishingWatchServiceTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<GlobalFishingWatchService>>();
            _service = new GlobalFishingWatchService(_mockHttpClientFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetVesselTypeAsync_ShouldReturnVesselType_WhenApiCallIsSuccessful()
        {
            // Arrange
            var imo = 1234567;
            var expectedVesselType = "cargo";
            var jsonResponse = $@"{{""entries"":[{{""type"":""{expectedVesselType}""}}]}}";

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse),
                });

            var client = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://gateway.api.globalfishingwatch.org/")
            };
            _mockHttpClientFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(client);

            // Act
            var result = await _service.GetVesselTypeAsync(imo);

            // Assert
            Assert.Equal(expectedVesselType, result);
        }
    }
}
