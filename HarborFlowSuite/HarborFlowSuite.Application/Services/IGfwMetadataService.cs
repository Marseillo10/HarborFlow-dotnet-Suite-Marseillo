using System.Threading.Tasks;
using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Application.Services
{
    public interface IGfwMetadataService
    {
        Task<VesselMetadataDto> GetVesselMetadataAsync(string mmsi);
        Task<VesselPositionUpdateDto?> GetVesselPositionAsync(string mmsi);
    }
}
