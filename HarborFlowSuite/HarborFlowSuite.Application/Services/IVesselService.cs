using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Application.Services
{
    public interface IVesselService
    {
        Task<List<Vessel>> GetVessels();
        Task<Vessel> GetVesselById(Guid id);
        Task<List<VesselPositionDto>> GetVesselPositions();
        Task<Vessel> CreateVessel(Vessel vessel);
        Task<Vessel> UpdateVessel(Vessel vessel);
        Task<bool> DeleteVessel(Guid id);
    }
}
