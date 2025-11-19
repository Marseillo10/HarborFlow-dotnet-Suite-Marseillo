using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class VesselService : IVesselService
    {
        private readonly ApplicationDbContext _context;

        public VesselService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Vessel>> GetVessels()
        {
            return await _context.Vessels.ToListAsync();
        }

        public async Task<List<VesselPositionDto>> GetVesselPositions()
        {
            var positions = await _context.Vessels
                .Select(v => v.VesselPositions.OrderByDescending(vp => vp.RecordedAt).FirstOrDefault())
                .Where(vp => vp != null)
                .Select(vp => new VesselPositionDto
                {
                    VesselId = vp.VesselId.ToString(),
                    VesselName = vp.Vessel.Name,
                    VesselType = vp.Vessel.VesselType,
                    Latitude = vp.Latitude,
                    Longitude = vp.Longitude,
                    Heading = vp.Heading,
                    Speed = vp.Speed,
                    RecordedAt = vp.RecordedAt
                })
                .ToListAsync();

            return positions;
        }
    }
}
