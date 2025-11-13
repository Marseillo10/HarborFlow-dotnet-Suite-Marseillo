using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VesselController : ControllerBase
    {
        [HttpGet]
        public Task<ActionResult<List<Vessel>>> GetVessels()
        {
            var vessels = new List<Vessel>
            {
                new Vessel { Id = Guid.NewGuid(), Name = "Ever Given", ImoNumber = "9811000", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "Majestic Maersk", ImoNumber = "9619907", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "MSC Gulsun", ImoNumber = "9839430", VesselType = "Container Ship", IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "HMM Algeciras", ImoNumber = "9863297", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Vessel { Id = Guid.NewGuid(), Name = "CMA CGM Jacques Saade", ImoNumber = "9839173", VesselType = "Container Ship", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };
            return Task.FromResult<ActionResult<List<Vessel>>>(Ok(vessels));
        }

        [HttpGet("positions")]
        public Task<ActionResult<List<VesselPositionDto>>> GetVesselPositions()
        {
            var positions = new List<VesselPositionDto>
            {
                new VesselPositionDto { VesselId = Guid.NewGuid().ToString(), VesselName = "Wanderer", VesselType = "Cargo", Latitude = 1.25m, Longitude = 103.9m, Heading = 110, Speed = 12, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid().ToString(), VesselName = "Odyssey", VesselType = "Tanker", Latitude = 1.22m, Longitude = 103.85m, Heading = 240, Speed = 10, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid().ToString(), VesselName = "Voyager", VesselType = "Passenger", Latitude = 1.28m, Longitude = 103.95m, Heading = 90, Speed = 20, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid().ToString(), VesselName = "Sea Spirit", VesselType = "Fishing", Latitude = 1.20m, Longitude = 104.0m, Heading = 180, Speed = 7, RecordedAt = DateTime.UtcNow },
                new VesselPositionDto { VesselId = Guid.NewGuid().ToString(), VesselName = "Wind Dancer", VesselType = "Sailing", Latitude = 1.26m, Longitude = 103.8m, Heading = 310, Speed = 5, RecordedAt = DateTime.UtcNow }
            };
            return Task.FromResult<ActionResult<List<VesselPositionDto>>>(Ok(positions));
        }
    }
}