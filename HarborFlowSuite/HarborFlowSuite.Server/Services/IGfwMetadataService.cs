using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Server.Services
{
    public interface IGfwMetadataService
    {
        Task<VesselMetadataDto> GetVesselMetadataAsync(string mmsi);
    }
}
