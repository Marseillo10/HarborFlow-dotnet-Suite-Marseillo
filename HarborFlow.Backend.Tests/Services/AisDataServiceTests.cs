using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Infrastructure.DTOs;
using HarborFlow.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace HarborFlow.Backend.Tests.Services
{
    public class AisDataServiceTests
    {
        private readonly Mock<ILogger<AisDataService>> _loggerMock;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;

        public AisDataServiceTests()
        {
            _loggerMock = new Mock<ILogger<AisDataService>>();
            _handlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_handlerMock.Object);
        }

        private IConfiguration GetConfiguration(string? apiKey)
        {
            var configData = new Dictionary<string, string?>
            {
                { "ApiKeys:GlobalFishingWatch", apiKey }
            };
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        private void SetupHttpResponse(HttpStatusCode statusCode, string content)
        {
            _handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = statusCode,
                   Content = new StringContent(content)
               });
        }

        [Fact]
        public async Task GetVesselDataAsync_WithValidImo_ShouldReturnMappedVessel()
        {
            // Arrange
            var config = GetConfiguration("valid-key");
            var gfwResponse = new GfwVesselResponse
            {
                Entries = new List<GfwVessel>
                {
                    new GfwVessel { Imo = "1234567", Mmsi = "112233445", ShipName = "Test Vessel", Flag = "US", VesselType = "Fishing" }
                }
            };
            var jsonResponse = JsonSerializer.Serialize(gfwResponse);
            SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

            var service = new AisDataService(_httpClient, config, _loggerMock.Object);

            // Act
            var result = await service.GetVesselDataAsync("1234567");

            // Assert
            result.Should().NotBeNull();
            result.IMO.Should().Be("1234567");
            result.Name.Should().Be("Test Vessel");
            result.VesselType.Should().Be(Core.Models.VesselType.Fishing);
        }

        [Fact]
        public async Task GetVesselDataAsync_WithMissingApiKey_ShouldReturnPlaceholderVessel()
        {
            // Arrange
            var config = GetConfiguration(null);
            var service = new AisDataService(_httpClient, config, _loggerMock.Object);

            // Act
            var result = await service.GetVesselDataAsync("1234567");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("Not Configured");
        }

        [Fact]
        public async Task GetVesselDataAsync_WhenApiReturnsNoEntries_ShouldReturnPlaceholderVessel()
        {
            // Arrange
            var config = GetConfiguration("valid-key");
            var gfwResponse = new GfwVesselResponse { Entries = new List<GfwVessel>() };
            var jsonResponse = JsonSerializer.Serialize(gfwResponse);
            SetupHttpResponse(HttpStatusCode.OK, jsonResponse);

            var service = new AisDataService(_httpClient, config, _loggerMock.Object);

            // Act
            var result = await service.GetVesselDataAsync("1234567");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("Not Found");
        }

        [Fact]
        public async Task GetVesselDataAsync_WhenApiThrowsHttpError_ShouldReturnPlaceholderVessel()
        {
            // Arrange
            var config = GetConfiguration("valid-key");
            _handlerMock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>()).ThrowsAsync(new HttpRequestException());
            var service = new AisDataService(_httpClient, config, _loggerMock.Object);

            // Act
            var result = await service.GetVesselDataAsync("1234567");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("API Error");
        }

        [Fact]
        public async Task GetVesselDataAsync_WithInvalidJson_ShouldReturnPlaceholderVessel()
        {
            // Arrange
            var config = GetConfiguration("valid-key");
            SetupHttpResponse(HttpStatusCode.OK, "{invalid_json");
            var service = new AisDataService(_httpClient, config, _loggerMock.Object);

            // Act
            var result = await service.GetVesselDataAsync("1234567");

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Contain("API Error");
        }
    }
}
