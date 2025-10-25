using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HarborFlow.Infrastructure.Services
{
    public class AisDataService : IAisDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AisDataService> _logger;

        public AisDataService(HttpClient httpClient, IConfiguration configuration, ILogger<AisDataService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<Vessel> GetVesselDataAsync(string imo)
        {
            var apiKey = _configuration["ApiKeys:GlobalFishingWatch"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("Global Fishing Watch API key is not configured. Returning placeholder data.");
                return CreatePlaceholderVessel(imo);
            }

            // Assumption: The API base URL is 'https://api.globalfishingwatch.org' and the version is 'v2'.
            // This may need to be adjusted based on the official documentation.
            var requestUrl = $"https://api.globalfishingwatch.org/v2/vessels?query={imo}";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var gfwResponse = JsonSerializer.Deserialize<GfwVesselResponse>(content);

                var vesselData = gfwResponse?.Entries?.FirstOrDefault();

                if (vesselData == null)
                {
                    _logger.LogWarning($"No vessel found for IMO {imo} from Global Fishing Watch.");
                    return CreatePlaceholderVessel(imo, "Not Found");
                }

                // Map the DTO to the domain model
                var vessel = new Vessel
                {
                    IMO = vesselData.Imo ?? imo,
                    Mmsi = vesselData.Mmsi ?? "N/A",
                    Name = vesselData.ShipName ?? "N/A",
                    FlagState = vesselData.Flag ?? "N/A",
                    // GFW API provides a general 'vesselType'. We need to map it to our enum.
                    // This is a simple example; a more robust mapping might be needed.
                    VesselType = Enum.TryParse<VesselType>(vesselData.VesselType, true, out var type) ? type : VesselType.Other,
                    Positions = new List<VesselPosition>(),
                    UpdatedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow // This should ideally be set only on creation
                };

                return vessel;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP request to Global Fishing Watch API failed for IMO {imo}.");
                return CreatePlaceholderVessel(imo, "API Error");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Failed to deserialize response from Global Fishing Watch API for IMO {imo}.");
                return CreatePlaceholderVessel(imo, "API Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred while fetching vessel data for IMO {imo}.");
                return CreatePlaceholderVessel(imo, "Error");
            }
        }

        private Vessel CreatePlaceholderVessel(string imo, string status = "Not Configured")
        {
            return new Vessel
            {
                IMO = imo,
                Mmsi = "N/A",
                Name = $"N/A ({status})",
                Positions = new List<VesselPosition>(),
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}