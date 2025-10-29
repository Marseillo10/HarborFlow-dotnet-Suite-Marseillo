using HarborFlow.Infrastructure.Services;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using HarborFlow.Core.Models;
using System.Text.Json;
using HarborFlow.Infrastructure.DTOs.GlobalFishingWatch;

namespace HarborFlow.Tests.Services
{
    public class GlobalFishingWatchServiceTests
    {
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<GlobalFishingWatchService>> _mockLogger;
        private readonly GlobalFishingWatchService _service;

        public GlobalFishingWatchServiceTests()
        {
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<GlobalFishingWatchService>>();

            _mockConfiguration.Setup(c => c["ApiKeys:GlobalFishingWatch"]).Returns("test-api-key");

            _service = new GlobalFishingWatchService(_httpClient, _mockConfiguration.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetVesselTypeAsync_ShouldReturnVesselType_WhenApiCallIsSuccessful()
        {
            // Arrange
            var imo = "1234567";
            var expectedVesselType = VesselType.Cargo;
            var jsonResponse = JsonSerializer.Serialize(new GfwVesselResponse
            {
                Entries = new List<Entry>
                {
                    new Entry
                    {
                        Imo = imo,
                        SelfReportedInfo = new List<SelfReportedInfo>
                        {
                            new SelfReportedInfo { ShipType = "Cargo" }
                        }
                    }
                }
            });

            _mockHttpMessageHandler.Protected()
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

            // Act
            var result = await _service.GetVesselTypeAsync(imo);

            // Assert
            Assert.Equal(expectedVesselType, result);
        }
    }
}
