using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using HarborFlow.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq.Protected;

namespace HarborFlow.Tests.Services
{
    public class VesselTrackingServiceTests
    {
        private readonly Mock<HarborFlowDbContext> _mockContext;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<VesselTrackingService>> _mockLogger;
        private readonly VesselTrackingService _service;

        public VesselTrackingServiceTests()
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: "HarborFlowTestDb")
                .Options;
            _mockContext = new Mock<HarborFlowDbContext>(options);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<VesselTrackingService>>();

            _service = new VesselTrackingService(
                _mockContext.Object,
                _mockConfiguration.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task StartTracking_ShouldNotThrowException_WhenApiKeyIsMissing()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["ApiKeys:AisStream"]).Returns("YOUR_API_KEY");
            var boundingBoxes = new[] { new[] { -90.0, -180.0 }, new[] { 90.0, 180.0 } };

            // Act
            var exception = await Record.ExceptionAsync(() => _service.StartTracking(boundingBoxes));

            // Assert
            Assert.Null(exception);
        }
    }
}
