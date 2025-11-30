using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class VesselService : IVesselService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGfwMetadataService _gfwMetadataService;
        private readonly IAisDataService _aisDataService;

        public VesselService(ApplicationDbContext context, IGfwMetadataService gfwMetadataService, IAisDataService aisDataService)
        {
            _context = context;
            _gfwMetadataService = gfwMetadataService;
            _aisDataService = aisDataService;
        }

        public IEnumerable<VesselPositionUpdateDto> GetActiveVessels()
        {
            return _aisDataService.GetActiveVessels();
        }

        public async Task<List<Vessel>> GetVessels(string firebaseUid)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null) return new List<Vessel>();

            var query = _context.Vessels.AsQueryable();

            if (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest")
            {
                if (user.CompanyId.HasValue)
                {
                    query = query.Where(v => v.CompanyId == user.CompanyId.Value);
                }
                else
                {
                    return new List<Vessel>();
                }
            }

            return await query.ToListAsync();
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

        public async Task<VesselPositionDto?> GetVesselPosition(string mmsi)
        {
            // 1. Check DB for recent position (e.g. last 1 hour)
            var vessel = await _context.Vessels
                .Include(v => v.VesselPositions)
                .FirstOrDefaultAsync(v => v.MMSI == mmsi);

            if (vessel != null)
            {
                var recentPosition = vessel.VesselPositions
                    .OrderByDescending(vp => vp.RecordedAt)
                    .FirstOrDefault();

                if (recentPosition != null && recentPosition.RecordedAt > DateTime.UtcNow.AddHours(-1))
                {
                    return new VesselPositionDto
                    {
                        VesselId = vessel.Id.ToString(),
                        VesselName = vessel.Name,
                        VesselType = vessel.VesselType,
                        Latitude = recentPosition.Latitude,
                        Longitude = recentPosition.Longitude,
                        Heading = recentPosition.Heading,
                        Speed = recentPosition.Speed,
                        RecordedAt = recentPosition.RecordedAt,
                        IMO = vessel.ImoNumber,
                        VesselStatus = "Active"
                    };
                }
            }

            // 2. Fallback to GFW
            var gfwPosition = await _gfwMetadataService.GetVesselPositionAsync(mmsi);
            if (gfwPosition != null)
            {
                return new VesselPositionDto
                {
                    VesselId = vessel?.Id.ToString() ?? mmsi, // Use MMSI if vessel not in DB
                    VesselName = gfwPosition.Name,
                    VesselType = gfwPosition.VesselType,
                    Latitude = (decimal)gfwPosition.Latitude,
                    Longitude = (decimal)gfwPosition.Longitude,
                    Heading = (decimal)gfwPosition.Heading,
                    Speed = (decimal)gfwPosition.Speed,
                    RecordedAt = DateTime.UtcNow, // GFW doesn't give us time in this simple DTO
                    IMO = gfwPosition.Metadata?.ImoNumber ?? "",
                    VesselStatus = "GFW Tracked"
                };
            }

            return null;
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
