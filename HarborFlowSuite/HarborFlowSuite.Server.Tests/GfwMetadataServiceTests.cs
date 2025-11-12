using Moq;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HarborFlowSuite.Server.Services;
using HarborFlowSuite.Core.DTOs;
using System.Text.Json;
using System.Net;
using System.Threading;
using Moq.Protected;
using System.Collections.Concurrent;

namespace HarborFlowSuite.Server.Tests
{
    public class GfwMetadataServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly GfwMetadataService _gfwMetadataService;

        public GfwMetadataServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["GfwApiKey"]).Returns("test_gfw_api_key");

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _gfwMetadataService = new GfwMetadataService(_httpClient, _mockConfiguration.Object);
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsCachedData_WhenAvailable()
        {
            // Arrange
            var mmsi = "123456789";
            var gfwApiResponse = new { Entries = new[] { new { Flag = "USA", LengthM = 100.0, Imo = "IMO123" } } };
            var jsonResponse = JsonSerializer.Serialize(gfwApiResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result1 = await _gfwMetadataService.GetVesselMetadataAsync(mmsi); // First call, should fetch and cache
            var result2 = await _gfwMetadataService.GetVesselMetadataAsync(mmsi); // Second call, should be cached

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(result1.ImoNumber, result2.ImoNumber);

            // Verify HTTP call was made only once
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains($"mmsi={mmsi}")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetVesselMetadataAsync_FetchesAndCachesData_WhenNotAvailable()
        {
            // Arrange
            var mmsi = "987654321";
            var gfwApiResponse = new { Entries = new[] { new { Flag = "GBR", LengthM = 150.5, Imo = "IMO456" } } };
            var jsonResponse = JsonSerializer.Serialize(gfwApiResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains($"mmsi={mmsi}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await _gfwMetadataService.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GBR", result.Flag);
            Assert.Equal(150.5, result.Length);
            Assert.Equal("IMO456", result.ImoNumber);

            // Verify HTTP call was made
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains($"mmsi={mmsi}")),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsNull_OnApiError()
        {
            // Arrange
            var mmsi = "111222333";
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError
                });

            // Act
            var result = await _gfwMetadataService.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsNull_WhenApiKeyIsNotConfigured()
        {
            // Arrange
            var mmsi = "123456789";
            _mockConfiguration.Setup(c => c["GfwApiKey"]).Returns((string)null); // No API key

            var serviceWithoutApiKey = new GfwMetadataService(_httpClient, _mockConfiguration.Object);

            // Act
            var result = await serviceWithoutApiKey.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.Null(result);
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
