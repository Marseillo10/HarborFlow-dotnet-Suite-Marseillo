
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.DTOs;
using Microsoft.Extensions.Configuration;

namespace HarborFlow.Infrastructure.Services
{
    public class AisDataService : IAisDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AisDataService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<VesselFinderResponse> GetVesselDataAsync(string imo)
        {
            var apiKey = _configuration["ApiKeys:VesselFinder"];
            var url = $"https://api.vesselfinder.com/vessels?userkey={apiKey}&imo={imo}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VesselFinderResponse>(content);
        }
    }
}
