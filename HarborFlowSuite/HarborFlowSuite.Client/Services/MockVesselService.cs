using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class MockVesselService : IVesselService
    {
        public Task<List<Vessel>> GetVessels()
        {
            var vessels = new List<Vessel>
            {
                new Vessel { Id = Guid.NewGuid(), Name = "Ever Given", ImoNumber = "9811000", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "Majestic Maersk", ImoNumber = "9619907", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "MSC Gulsun", ImoNumber = "9839430", VesselType = "Container Ship", IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "HMM Algeciras", ImoNumber = "9863297", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "CMA CGM Jacques Saade", ImoNumber = "9839173", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            return Task.FromResult(vessels);
        }

        public Task<List<VesselPositionDto>> GetVesselPositions()
        {
            var positions = new List<VesselPositionDto>
            {
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "Wanderer", VesselType = "Cargo", Latitude = -7.797068m + 0.01m, Longitude = 110.370529m + 0.01m, Heading = 45, Speed = 10, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "Odyssey", VesselType = "Tanker", Latitude = -7.797068m - 0.01m, Longitude = 110.370529m - 0.01m, Heading = 180, Speed = 15, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "Voyager", VesselType = "Passenger", Latitude = -7.797068m, Longitude = 110.370529m + 0.02m, Heading = 270, Speed = 12, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid(), VesselName = "Sea Spirit", VesselType = "Fishing", Latitude = -7.797068m + 0.02m, Longitude = 110.370529m - 0.02m, Heading = 120, Speed = 8, RecordedAt = DateTime.UtcNow }
            };
            return Task.FromResult(positions);
        }
    }
}
