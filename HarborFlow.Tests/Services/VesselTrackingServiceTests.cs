using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using HarborFlow.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace HarborFlow.Tests.Services
{
    public class VesselTrackingServiceTests
    {
        private readonly Mock<HarborFlowDbContext> _mockContext;
        private readonly Mock<ILogger<VesselTrackingService>> _mockLogger;
        private readonly Mock<IAisStreamService> _mockAisStreamService;
        private readonly VesselTrackingService _service;

        public VesselTrackingServiceTests()
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: "HarborFlowTestDb_VesselTracking")
                .Options;
            _mockContext = new Mock<HarborFlowDbContext>(options);
            _mockLogger = new Mock<ILogger<VesselTrackingService>>();
            _mockAisStreamService = new Mock<IAisStreamService>();

            _service = new VesselTrackingService(
                _mockContext.Object,
                _mockLogger.Object,
                _mockAisStreamService.Object);
        }

        [Fact]
        public async Task StartTracking_ShouldCallAisStreamServiceStart()
        {
            // Arrange
            var boundingBoxes = new[] { new[] { -90.0, -180.0 }, new[] { 90.0, 180.0 } };

            // Act
            await _service.StartTracking(boundingBoxes);

            // Assert
            _mockAisStreamService.Verify(s => s.Start(), Times.Once);
        }
    }
}