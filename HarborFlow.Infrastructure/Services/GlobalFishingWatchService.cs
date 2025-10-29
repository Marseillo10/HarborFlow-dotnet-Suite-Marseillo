using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Infrastructure.Services
{
    public class GlobalFishingWatchService : IGlobalFishingWatchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GlobalFishingWatchService> _logger;

        public GlobalFishingWatchService(HttpClient httpClient, IConfiguration configuration, ILogger<GlobalFishingWatchService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
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

        public async Task<Dictionary<string, VesselType>> GetVesselTypesAsync(List<string> imos)
        {
            var result = new Dictionary<string, VesselType>();
            if (!imos.Any()) return result;

            var apiKey = _configuration["ApiKeys:GlobalFishingWatch"];
            var imoString = string.Join(",", imos);
            var requestUri = $"https://gateway.api.globalfishingwatch.org/v3/vessels/search?query={imoString}&datasets[0]=public-global-vessel-identity:latest";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var vesselData = JsonSerializer.Deserialize<HarborFlow.Infrastructure.DTOs.GlobalFishingWatch.GfwVesselResponse>(content);
                if (vesselData?.Entries != null)
                {
                    foreach (var entry in vesselData.Entries)
                    {
                        if (entry.SelfReportedInfo?.Count > 0)
                        {
                            var shipType = entry.SelfReportedInfo[0].ShipType;
                            result[entry.Imo] = MapToVesselType(shipType);
                        }
                        else
                        {
                            result[entry.Imo] = VesselType.Other;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vessel types for IMOs {Imos}", imoString);
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
