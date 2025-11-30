using System.Collections.Generic;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface IAisDataService
    {
        IEnumerable<VesselPositionUpdateDto> GetActiveVessels();
    }
}
