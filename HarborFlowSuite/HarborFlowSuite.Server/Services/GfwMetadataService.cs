using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using HarborFlowSuite.Shared.DTOs;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Core.Models;
using System.Linq;

namespace HarborFlowSuite.Server.Services
{
    public class GfwMetadataService : IGfwMetadataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _gfwApiKey;
        private readonly ApplicationDbContext _context;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(7);

        public GfwMetadataService(HttpClient httpClient, IConfiguration configuration, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _gfwApiKey = configuration["GfwApiKey"];
            _context = context;
            // The Authorization header will be set in Program.cs
        }

        public async Task<VesselMetadataDto> GetVesselMetadataAsync(string mmsi)
        {
            if (string.IsNullOrEmpty(_gfwApiKey))
            {
                // Return null or a default object if the API key is not configured
                return null;
            }

            var cachedMetadata = await _context.GfwMetadataCache
                .FirstOrDefaultAsync(c => c.Mmsi == mmsi);

            if (cachedMetadata != null && DateTime.UtcNow - cachedMetadata.LastUpdated < _cacheExpiration)
            {
                return new VesselMetadataDto
                {
                    Flag = cachedMetadata.Flag,
                    Length = cachedMetadata.Length,
                    ImoNumber = cachedMetadata.ImoNumber,
                    ShipName = cachedMetadata.ShipName,
                    Callsign = cachedMetadata.Callsign,
                    Geartype = cachedMetadata.Geartype
                };
            }

            try
            {
                // GFW Vessels API endpoint for searching by MMSI
                // The base address is set in Program.cs, so we use a relative URL here
                var apiUrl = $"vessels/search?query={mmsi}&datasets[0]=public-global-vessel-identity:latest";

                var response = await _httpClient.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    // Handle non-success status codes (e.g., 404 Not Found)
                    return null;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var gfwVesselResponse = JsonSerializer.Deserialize<GfwVesselSearchResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (gfwVesselResponse?.Entries != null && gfwVesselResponse.Entries.Any())
                {
                    var vesselEntry = gfwVesselResponse.Entries.First();
                    var metadata = new VesselMetadataDto
                    {
                        Flag = vesselEntry.Flag,
                        Length = (double)(vesselEntry.LengthM ?? 0.0),
                        ImoNumber = vesselEntry.Imo,
                        ShipName = vesselEntry.Shipname,
                        Callsign = vesselEntry.Callsign,
                        Geartype = vesselEntry.Geartype
                    };

                    if (cachedMetadata != null)
                    {
                        cachedMetadata.Flag = metadata.Flag;
                        cachedMetadata.Length = metadata.Length;
                        cachedMetadata.ImoNumber = metadata.ImoNumber;
                        cachedMetadata.ShipName = metadata.ShipName;
                        cachedMetadata.Callsign = metadata.Callsign;
                        cachedMetadata.Geartype = metadata.Geartype;
                        cachedMetadata.LastUpdated = DateTime.UtcNow;
                    }
                    else
                    {
                        _context.GfwMetadataCache.Add(new GfwMetadataCache
                        {
                            Mmsi = mmsi,
                            Flag = metadata.Flag,
                            Length = metadata.Length,
                            ImoNumber = metadata.ImoNumber,
                            ShipName = metadata.ShipName,
                            Callsign = metadata.Callsign,
                            Geartype = metadata.Geartype,
                            LastUpdated = DateTime.UtcNow
                        });
                    }

                    await _context.SaveChangesAsync();

                    return metadata;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching GFW metadata for MMSI {mmsi}: {ex.Message}");
                return null;
            }
        }

        // Helper classes to deserialize the GFW API response
        private class GfwVesselSearchResponse
        {
            public List<GfwVesselEntry> Entries { get; set; }
        }

        private class GfwVesselEntry
        {
            public string Flag { get; set; }
            public double? LengthM { get; set; }
            public string Imo { get; set; }
            public string Shipname { get; set; }
            public string Callsign { get; set; }
            public string Geartype { get; set; }
        }
    }
}
