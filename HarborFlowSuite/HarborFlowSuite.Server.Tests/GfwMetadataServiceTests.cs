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
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Core.Models;
using System;
using System.Collections.Generic;

namespace HarborFlowSuite.Server.Tests
{
    public class GfwMetadataServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public GfwMetadataServiceTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["GfwApiKey"]).Returns("test_gfw_api_key");
            _mockConfiguration.Setup(c => c["GfwApiBaseUrl"]).Returns("https://gfw.api.com/"); // Mock base URL

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://gfw.api.com/") // Set base address
            };
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var mockCurrentUserService = new Mock<HarborFlowSuite.Infrastructure.Services.ICurrentUserService>();
            return new ApplicationDbContext(options, mockCurrentUserService.Object);
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsCachedData_WhenAvailable()
        {
            // Arrange
            var mmsi = "123456789";
            var cachedEntry = new GfwMetadataCache
            {
                Mmsi = mmsi,
                Flag = "USA",
                Length = 100.0,
                ImoNumber = "IMO123",
                LastUpdated = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1))
            };

            var context = GetInMemoryDbContext();
            context.GfwMetadataCache.Add(cachedEntry);
            await context.SaveChangesAsync();

            var service = new GfwMetadataService(_httpClient, _mockConfiguration.Object, context);

            // Act
            var result = await service.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("USA", result.Flag);
            Assert.Equal(100.0, result.Length);
            Assert.Equal("IMO123", result.ImoNumber);

            // Verify no HTTP call was made
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Never(),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task GetVesselMetadataAsync_FetchesAndCachesData_WhenNotAvailable()
        {
            // Arrange
            var mmsi = "987654321";
            var context = GetInMemoryDbContext();
            var service = new GfwMetadataService(_httpClient, _mockConfiguration.Object, context);

            var gfwApiResponse = new { Entries = new[] { new { Flag = "GBR", LengthM = 150.5, Imo = "IMO456" } } };
            var jsonResponse = JsonSerializer.Serialize(gfwApiResponse);

            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains($"query={mmsi}")),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse)
                });

            // Act
            var result = await service.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("GBR", result.Flag);
            Assert.Equal(150.5, result.Length);
            Assert.Equal("IMO456", result.ImoNumber);

            // Verify HTTP call was made
            _mockHttpMessageHandler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains($"query={mmsi}")),
                ItExpr.IsAny<CancellationToken>()
            );

            // Verify that the data was cached
            var cachedData = await context.GfwMetadataCache.FirstOrDefaultAsync(c => c.Mmsi == mmsi);
            Assert.NotNull(cachedData);
            Assert.Equal("GBR", cachedData.Flag);
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsNull_OnApiError()
        {
            // Arrange
            var mmsi = "111222333";
            var context = GetInMemoryDbContext();
            var service = new GfwMetadataService(_httpClient, _mockConfiguration.Object, context);

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
            var result = await service.GetVesselMetadataAsync(mmsi);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetVesselMetadataAsync_ReturnsNull_WhenApiKeyIsNotConfigured()
        {
            // Arrange
            var mmsi = "123456789";
            _mockConfiguration.Setup(c => c["GfwApiKey"]).Returns((string)null); // No API key
            var context = GetInMemoryDbContext();
            var serviceWithoutApiKey = new GfwMetadataService(_httpClient, _mockConfiguration.Object, context);

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
