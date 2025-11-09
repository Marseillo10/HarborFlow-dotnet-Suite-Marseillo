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
    }
}