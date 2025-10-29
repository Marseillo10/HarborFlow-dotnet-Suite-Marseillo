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

        public async Task<Dictionary<string, VesselType>> GetVesselTypesAsync(List<string> imos)
        {
            var result = new Dictionary<string, VesselType>();
            if (!imos.Any()) return result;

            var client = _httpClientFactory.CreateClient("GlobalFishingWatch");
            try
            {
                var imoString = string.Join(",", imos);
                var response = await client.GetAsync($"vessels?imos={imoString}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var vesselData = JsonConvert.DeserializeObject<GfwVesselResponse>(content);
                if (vesselData?.Entries != null)
                {
                    foreach (var entry in vesselData.Entries)
                    {
                        if (Enum.TryParse<VesselType>(entry.Type, true, out var vesselType))
                        {
                            result[entry.Imo.ToString()] = vesselType;
                        }
                        else
                        {
                            result[entry.Imo.ToString()] = VesselType.Other;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vessel types for IMOs {Imos}", string.Join(",", imos));
            }

            // For any IMOs not found in the API response, default to Other
            foreach (var imo in imos)
            {
                if (!result.ContainsKey(imo))
                {
                    result[imo] = VesselType.Other;
                }
            }

            return result;
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
