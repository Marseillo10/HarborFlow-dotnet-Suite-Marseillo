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

        public async Task<Vessel> GetVesselById(Guid id)
        {
            return await _context.Vessels.FindAsync(id);
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

        public async Task<Vessel> CreateVessel(Vessel vessel)
        {
            _context.Vessels.Add(vessel);
            await _context.SaveChangesAsync();
            return vessel;
        }

        public async Task<Vessel> UpdateVessel(Vessel vessel)
        {
            var existingVessel = await _context.Vessels.FindAsync(vessel.Id);
            if (existingVessel == null)
            {
                return null;
            }

            existingVessel.Name = vessel.Name;
            existingVessel.ImoNumber = vessel.ImoNumber;
            existingVessel.VesselType = vessel.VesselType;
            existingVessel.Length = vessel.Length;
            existingVessel.Width = vessel.Width;
            existingVessel.IsActive = vessel.IsActive;
            // Update other properties as needed

            await _context.SaveChangesAsync();
            return existingVessel;
        }
        public async Task<bool> DeleteVessel(Guid id)
        {
            var vessel = await _context.Vessels.FindAsync(id);
            if (vessel == null)
            {
                return false;
            }

            _context.Vessels.Remove(vessel);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
