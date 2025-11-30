using HarborFlowSuite.Shared.DTOs;
using System.Net.Http.Headers;
using System.Text.Json;

namespace HarborFlowSuite.Server.Services
{
    public class GfwApiService : IGfwApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GfwApiService> _logger;
        private readonly string _apiKey;

        public GfwApiService(HttpClient httpClient, IConfiguration configuration, ILogger<GfwApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["GfwApiKey"];

            var baseUrl = configuration["GfwApiBaseUrl"] ?? "https://gateway.api.globalfishingwatch.org/v3/";
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<VesselMetadataDto?> GetVesselIdentityAsync(string mmsi)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                _logger.LogWarning("GFW API Key is missing. Skipping GFW lookup.");
                return null;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"vessels/search?query=mmsi:{mmsi}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Common for vessels not in GFW database or invalid MMSI format. Log at Debug to reduce noise.
                        _logger.LogDebug("GFW API lookup skipped for MMSI {MMSI}: {StatusCode}", mmsi, response.StatusCode);
                    }
                    else
                    {
                        _logger.LogWarning("GFW API call failed for MMSI {MMSI}: {StatusCode}", mmsi, response.StatusCode);
                    }
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("entries", out var entries) && entries.GetArrayLength() > 0)
                {
                    // Get the first match
                    var entry = entries[0];

                    var metadata = new VesselMetadataDto();

                    if (entry.TryGetProperty("shipname", out var nameProp))
                        metadata.ShipName = nameProp.GetString();

                    if (entry.TryGetProperty("flag", out var flagProp))
                        metadata.Flag = flagProp.GetString();

                    if (entry.TryGetProperty("imo", out var imoProp))
                        metadata.ImoNumber = imoProp.GetString();

                    if (entry.TryGetProperty("callsign", out var callsignProp))
                        metadata.Callsign = callsignProp.GetString();

                    // GFW often has dimensions in registry info or characteristics
                    // This is a simplified mapping based on common GFW response structure
                    // We might need to adjust based on actual response if nested
                    if (entry.TryGetProperty("registryInfo", out var registryInfo))
                    {
                        if (registryInfo.TryGetProperty("lengthM", out var lenProp))
                            metadata.Length = lenProp.GetDouble();

                        if (registryInfo.TryGetProperty("tonnageGt", out var tonProp))
                            metadata.Geartype = $"GT: {tonProp.GetDouble()}"; // Storing tonnage in Geartype for now as we don't have a Tonnage field
                    }

                    // Map GFW vessel type to our Type field if available
                    if (entry.TryGetProperty("vesselType", out var typeProp))
                    {
                        metadata.VesselType = typeProp.GetString();
                    }

                    return metadata;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GFW data for MMSI {MMSI}", mmsi);
            }

            return null;
        }
    }
}
