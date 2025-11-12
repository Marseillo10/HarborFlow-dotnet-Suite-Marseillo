using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class VesselService : IVesselService
    {
        private readonly HttpClient _httpClient;

        public VesselService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Vessel>> GetVessels()
        {
            return await _httpClient.GetFromJsonAsync<List<Vessel>>("api/vessel");
        }

        public async Task<List<VesselPositionDto>> GetVesselPositions()
        {
            return await _httpClient.GetFromJsonAsync<List<VesselPositionDto>>("api/vessel/positions");
        }
    }
}
