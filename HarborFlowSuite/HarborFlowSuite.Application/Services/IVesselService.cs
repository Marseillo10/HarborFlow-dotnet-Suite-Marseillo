using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface IVesselService
    {
        Task<List<Vessel>> GetVessels(string firebaseUid);
        Task<Vessel> GetVesselById(Guid id, string firebaseUid);
        Task<List<VesselPositionDto>> GetVesselPositions(string firebaseUid);
        Task<VesselPositionDto?> GetVesselPosition(string mmsi, string firebaseUid, bool allowGfwFallback = true);
        IEnumerable<VesselPositionUpdateDto> GetActiveVessels(string firebaseUid);
        Task<Vessel> CreateVessel(Vessel vessel);
        Task<Vessel> UpdateVessel(Vessel vessel);
        Task<bool> DeleteVessel(Guid id);
    }
}
