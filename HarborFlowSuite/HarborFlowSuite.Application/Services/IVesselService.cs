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
        Task<Vessel> GetVesselById(Guid id);
        Task<List<VesselPositionDto>> GetVesselPositions();
        Task<VesselPositionDto?> GetVesselPosition(string mmsi);
        IEnumerable<VesselPositionUpdateDto> GetActiveVessels();
        Task<Vessel> CreateVessel(Vessel vessel);
        Task<Vessel> UpdateVessel(Vessel vessel);
        Task<bool> DeleteVessel(Guid id);
    }
}
