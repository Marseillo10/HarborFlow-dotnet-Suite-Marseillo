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

        public async Task<Vessel> GetVessel(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Vessel>($"api/vessel/{id}");
        }

        public async Task CreateVessel(Vessel vessel)
        {
            await _httpClient.PostAsJsonAsync("api/vessel", vessel);
        }

        public async Task UpdateVessel(Vessel vessel)
        {
            await _httpClient.PutAsJsonAsync($"api/vessel/{vessel.Id}", vessel);
        }

        public async Task DeleteVessel(Guid id)
        {
            await _httpClient.DeleteAsync($"api/vessel/{id}");
        }

        public async Task<List<VesselPositionDto>> GetVesselPositions()
        {
            return await _httpClient.GetFromJsonAsync<List<VesselPositionDto>>("api/vessel/positions");
        }

        public async Task<VesselPositionDto?> GetVesselPosition(string mmsi, bool allowGfwFallback = true)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<VesselPositionDto>($"api/vessel/positions/{mmsi}?allowGfwFallback={allowGfwFallback}");
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<IEnumerable<HarborFlowSuite.Shared.DTOs.VesselPositionUpdateDto>> GetActiveVessels()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<HarborFlowSuite.Shared.DTOs.VesselPositionUpdateDto>>("api/vessel/active")
                       ?? Enumerable.Empty<HarborFlowSuite.Shared.DTOs.VesselPositionUpdateDto>();
            }
            catch
            {
                return Enumerable.Empty<HarborFlowSuite.Shared.DTOs.VesselPositionUpdateDto>();
            }
        }
    }
}
