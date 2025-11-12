using System.Threading.Tasks;
using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Server.Services
{
    public interface IGfwMetadataService
    {
        Task<VesselMetadataDto> GetVesselMetadataAsync(string mmsi);
    }
}
