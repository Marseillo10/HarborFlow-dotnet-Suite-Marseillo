using System;
using System.Linq;
using System.Threading.Tasks;
using HarborFlow.Application.Services;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FluentAssertions;

namespace HarborFlow.Backend.Tests.Services
{
    public class VesselTrackingServiceTests
    {
        private HarborFlowDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<HarborFlowDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var context = new HarborFlowDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private ILogger<VesselTrackingService> GetLogger()
        {
            return new Mock<ILogger<VesselTrackingService>>().Object;
        }

        private IAisStreamService GetAisStreamService()
        {
            return new Mock<IAisStreamService>().Object;
        }

        [Fact]
        public async Task AddVesselAsync_ShouldCreateAndStoreVessel()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(AddVesselAsync_ShouldCreateAndStoreVessel));
            var service = new VesselTrackingService(dbContext, GetLogger(), GetAisStreamService());
            var newVessel = new Vessel { IMO = "9876543", Name = "Test Ship" };

            // Act
            var result = await service.AddVesselAsync(newVessel);

            // Assert
            result.Should().NotBeNull();
            result.IMO.Should().Be("9876543");

            var vesselInDb = await dbContext.Vessels.FindAsync("9876543");
            vesselInDb.Should().NotBeNull();
            vesselInDb.Name.Should().Be("Test Ship");
        }

        [Fact]
        public async Task AddVesselAsync_WithExistingImo_ShouldThrowException()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(AddVesselAsync_WithExistingImo_ShouldThrowException));
            var service = new VesselTrackingService(dbContext, GetLogger(), GetAisStreamService());
            var existingVessel = new Vessel { IMO = "9876543", Name = "Existing Ship" };
            await service.AddVesselAsync(existingVessel);

            var newVesselWithSameImo = new Vessel { IMO = "9876543", Name = "New Ship" };

            // Act
            Func<Task> act = async () => await service.AddVesselAsync(newVesselWithSameImo);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateVesselAsync_ShouldModifyExistingVessel()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(UpdateVesselAsync_ShouldModifyExistingVessel));
            var service = new VesselTrackingService(dbContext, GetLogger(), GetAisStreamService());
            var originalVessel = new Vessel { IMO = "1112223", Name = "Original Name" };
            await service.AddVesselAsync(originalVessel);

            var updatedVessel = new Vessel { IMO = "1112223", Name = "Updated Name" };

            // Act
            var result = await service.UpdateVesselAsync(updatedVessel);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Name");

            var vesselInDb = await dbContext.Vessels.FindAsync("1112223");
            vesselInDb.Should().NotBeNull();
            vesselInDb!.Name.Should().Be("Updated Name");
        }

        [Fact]
        public async Task DeleteVesselAsync_ShouldRemoveVesselFromDb()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(DeleteVesselAsync_ShouldRemoveVesselFromDb));
            var service = new VesselTrackingService(dbContext, GetLogger(), GetAisStreamService());
            var vessel = new Vessel { IMO = "3334445", Name = "To Be Deleted" };
            await service.AddVesselAsync(vessel);

            // Act
            await service.DeleteVesselAsync(vessel.IMO);

            // Assert
            var vesselInDb = await dbContext.Vessels.FindAsync(vessel.IMO);
            vesselInDb.Should().BeNull();
        }

        [Fact]
        public async Task SearchVesselsAsync_ShouldReturnMatchingVessels()
        {
            // Arrange
            var dbContext = GetDbContext(nameof(SearchVesselsAsync_ShouldReturnMatchingVessels));
            var service = new VesselTrackingService(dbContext, GetLogger(), GetAisStreamService());
            dbContext.Vessels.AddRange(
                new Vessel { IMO = "1000001", Name = "Alpha Star" },
                new Vessel { IMO = "1000002", Name = "Beta Star" },
                new Vessel { IMO = "1000003", Name = "Alpha Moon" }
            );
            await dbContext.SaveChangesAsync();

            // Act
            var result = await service.SearchVesselsAsync("Alpha");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(v => v.Name == "Alpha Star");
            result.Should().Contain(v => v.Name == "Alpha Moon");
        }
    }
}
