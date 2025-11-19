using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselService
    {
        Task<List<Vessel>> GetVessels();
        Task<Vessel> GetVessel(Guid id);
        Task CreateVessel(Vessel vessel);
        Task UpdateVessel(Vessel vessel);
        Task DeleteVessel(Guid id);
        Task<List<VesselPositionDto>> GetVesselPositions();
    }
}
