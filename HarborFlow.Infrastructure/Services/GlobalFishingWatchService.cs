using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class GlobalFishingWatchService : IGlobalFishingWatchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GlobalFishingWatchService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<VesselType> GetVesselTypeAsync(string imo)
        {
            var apiKey = _configuration["ApiKeys:GlobalFishingWatch"];
            var requestUri = $"https://gateway.api.globalfishingwatch.org/v3/vessels/search?query={imo}&datasets[0]=public-global-vessel-identity:latest";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gfwResponse = JsonSerializer.Deserialize<HarborFlow.Infrastructure.DTOs.GlobalFishingWatch.GfwVesselResponse>(content);

                if (gfwResponse?.Entries?.Count > 0 && gfwResponse.Entries[0].SelfReportedInfo?.Count > 0)
                {
                    var shipType = gfwResponse.Entries[0].SelfReportedInfo[0].ShipType;
                    return MapToVesselType(shipType);
                }
            }

            return VesselType.Other; // Default value
        }

        private VesselType MapToVesselType(string gfwShipType)
        {
            return gfwShipType?.ToLower() switch
            {
                "fishing" => VesselType.Fishing,
                "tanker" => VesselType.Tanker,
                "cargo" => VesselType.Cargo,
                "passenger" => VesselType.Passenger,
                "pleasure" => VesselType.PleasureCraft,
                "sailing" => VesselType.Sailing,
                "tug" => VesselType.Tug,
                _ => VesselType.Other,
            };
        }
    }
}
