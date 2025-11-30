using HarborFlowSuite.Shared.DTOs;

namespace HarborFlowSuite.Server.Services
{
    public interface IGfwApiService
    {
        Task<VesselMetadataDto?> GetVesselIdentityAsync(string mmsi);
    }
}
