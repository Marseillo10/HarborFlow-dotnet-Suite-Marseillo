using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IVesselService
    {
        Task<List<Vessel>> GetVessels();
        Task<List<VesselPositionDto>> GetVesselPositions();
    }
}
