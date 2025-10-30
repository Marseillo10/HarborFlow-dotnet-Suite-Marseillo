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
using Microsoft.Extensions.Caching.Memory;

namespace HarborFlow.Infrastructure.Services
{
    public class GlobalFishingWatchService : IGlobalFishingWatchService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GlobalFishingWatchService> _logger;
        private readonly IMemoryCache _cache;

        public GlobalFishingWatchService(HttpClient httpClient, IConfiguration configuration, ILogger<GlobalFishingWatchService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _cache = cache;
        }

        public async Task<VesselType> GetVesselTypeAsync(string imo)
        {
            var cacheKey = $"VesselType_{imo}";
            if (_cache.TryGetValue(cacheKey, out VesselType cachedVesselType))
            {
                _logger.LogInformation("Cache hit for IMO {Imo}", imo);
                return cachedVesselType;
            }

            _logger.LogInformation("Cache miss for IMO {Imo}. Fetching from API.", imo);
            var apiKey = _configuration["ApiKeys:GlobalFishingWatch"];
            var requestUri = $"https://gateway.api.globalfishingwatch.org/v3/vessels/search?query={imo}&datasets[0]=public-global-vessel-identity:latest";

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");

            var response = await _httpClient.SendAsync(request);
            var vesselType = VesselType.Other;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gfwResponse = JsonSerializer.Deserialize<HarborFlow.Infrastructure.DTOs.GlobalFishingWatch.GfwVesselResponse>(content);

                if (gfwResponse?.Entries?.Count > 0 && gfwResponse.Entries[0].SelfReportedInfo?.Count > 0)
                {
                    var shipType = gfwResponse.Entries[0].SelfReportedInfo[0].ShipType;
                    vesselType = MapToVesselType(shipType);
                }
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
            _cache.Set(cacheKey, vesselType, cacheEntryOptions);

            return vesselType;
        }

        public async Task<Dictionary<string, VesselType>> GetVesselTypesAsync(List<string> imos)
        {
            var result = new Dictionary<string, VesselType>();
            var imosToFetch = new List<string>();

            foreach (var imo in imos.Distinct())
            {
                var cacheKey = $"VesselType_{imo}";
                if (_cache.TryGetValue(cacheKey, out VesselType cachedVesselType))
                {
                    _logger.LogInformation("Cache hit for IMO {Imo} in batch request.", imo);
                    result[imo] = cachedVesselType;
                }
                else
                {
                    imosToFetch.Add(imo);
                }
            }

            if (!imosToFetch.Any())
            {
                _logger.LogInformation("All IMOs found in cache for batch request.");
                return result;
            }

            _logger.LogInformation("Cache miss for {Count} IMOs in batch request. Fetching from API.", imosToFetch.Count);
            var apiKey = _configuration["ApiKeys:GlobalFishingWatch"];
            var imoString = string.Join(",", imosToFetch);
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
                    if (vesselData.Entries.Count != imosToFetch.Count)
                    {
                        _logger.LogWarning("API returned {ApiCount} results for {RequestCount} IMOs.", vesselData.Entries.Count, imosToFetch.Count);
                    }

                    for (int i = 0; i < vesselData.Entries.Count; i++)
                    {
                        var entry = vesselData.Entries[i];
                        var imo = imosToFetch[i];
                        var vesselType = VesselType.Other;

                        if (entry.SelfReportedInfo?.Count > 0)
                        {
                            var shipType = entry.SelfReportedInfo[0].ShipType;
                            vesselType = MapToVesselType(shipType);
                        }

                        result[imo] = vesselType;

                        var cacheKey = $"VesselType_{imo}";
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                        _cache.Set(cacheKey, vesselType, cacheEntryOptions);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching vessel types for IMOs {Imos}", imoString);
            }

            // For any IMOs not found in the API response or that failed, default to Other
            foreach (var imo in imosToFetch)
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
