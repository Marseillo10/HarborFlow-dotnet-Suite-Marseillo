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

        public IEnumerable<VesselPositionUpdateDto> GetActiveVessels(string firebaseUid)
        {
            var allVessels = _aisDataService.GetActiveVessels();
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.FirebaseUid == firebaseUid);

            if (user != null && (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest"))
            {
                if (!user.CompanyId.HasValue) return Enumerable.Empty<VesselPositionUpdateDto>();

                var companyMmsis = _context.Vessels
                    .Where(v => v.CompanyId == user.CompanyId.Value)
                    .Select(v => v.MMSI)
                    .ToHashSet();

                return allVessels.Where(v => companyMmsis.Contains(v.MMSI));
            }

            return allVessels;
        }

        public async Task<List<Vessel>> GetVessels(string firebaseUid)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null) return new List<Vessel>();

            var query = _context.Vessels.Include(v => v.Company).AsQueryable();

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

        public async Task<Vessel> GetVesselById(Guid id, string firebaseUid)
        {
            var vessel = await _context.Vessels.FindAsync(id);
            if (vessel == null) return null;

            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user != null && (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest"))
            {
                if (vessel.CompanyId != user.CompanyId) return null;
            }

            return vessel;
        }

        public async Task<List<VesselPositionDto>> GetVesselPositions(string firebaseUid)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null) return new List<VesselPositionDto>();

            var query = _context.Vessels.AsQueryable();

            if (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest")
            {
                if (user.CompanyId.HasValue)
                {
                    query = query.Where(v => v.CompanyId == user.CompanyId.Value);
                }
                else
                {
                    return new List<VesselPositionDto>();
                }
            }

            var positions = await query
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

        public async Task<VesselPositionDto?> GetVesselPosition(string mmsi, string firebaseUid, bool allowGfwFallback = true)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            // 1. Check DB for recent position (e.g. last 1 hour)
            var vesselQuery = _context.Vessels
                .Include(v => v.VesselPositions)
                .Where(v => v.MMSI == mmsi);

            if (user != null && (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest"))
            {
                if (user.CompanyId.HasValue)
                {
                    vesselQuery = vesselQuery.Where(v => v.CompanyId == user.CompanyId.Value);
                }
                else
                {
                    return null;
                }
            }

            var vessel = await vesselQuery.FirstOrDefaultAsync();

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
            if (!allowGfwFallback)
            {
                return null;
            }

            // Only allow GFW fallback if user is NOT restricted (or if we decide GFW data is public)
            // For strict isolation, if it's not in their company DB, they shouldn't see it.
            if (user != null && (user.Role?.Name == "Vessel Agent" || user.Role?.Name == "Guest"))
            {
                return null;
            }

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
            // Check if a vessel with the same MMSI already exists
            var existingVessel = await _context.Vessels.FirstOrDefaultAsync(v => v.MMSI == vessel.MMSI);

            if (existingVessel != null)
            {
                // Update existing vessel details
                existingVessel.Name = vessel.Name;
                existingVessel.ImoNumber = vessel.ImoNumber;
                existingVessel.VesselType = vessel.VesselType;
                existingVessel.Length = vessel.Length;
                existingVessel.Width = vessel.Width;
                existingVessel.CompanyId = vessel.CompanyId;
                existingVessel.IsActive = vessel.IsActive;
                existingVessel.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingVessel;
            }

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
