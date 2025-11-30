using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PortsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Port>>> GetPorts(
            [FromQuery] double? minLat,
            [FromQuery] double? maxLat,
            [FromQuery] double? minLon,
            [FromQuery] double? maxLon)
        {
            var query = _context.Ports.AsQueryable();

            if (minLat.HasValue && maxLat.HasValue && minLon.HasValue && maxLon.HasValue)
            {
                query = query.Where(p =>
                    p.Latitude >= minLat.Value &&
                    p.Latitude <= maxLat.Value &&
                    p.Longitude >= minLon.Value &&
                    p.Longitude <= maxLon.Value);
            }

            // Limit results to prevent overloading the client if the viewport is too large
            // or if no bounds are provided (though client should provide them).
            // 500 ports is a reasonable limit for a single view.
            var ports = await query.Take(500).ToListAsync();

            return Ok(ports);
        }
    }
}
