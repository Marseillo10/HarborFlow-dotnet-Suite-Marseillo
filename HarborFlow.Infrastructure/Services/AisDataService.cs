using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure.DTOs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

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

        public async Task<Vessel> GetVesselDataAsync(string imo)
        {
            var apiKey = _configuration["ApiKeys:VesselFinder"] ?? _configuration["ApiKeys:AisStream"];
            var url = $"https://api.vesselfinder.com/vessels?userkey={apiKey}&imo={imo}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var vesselFinderResponse = JsonSerializer.Deserialize<VesselFinderResponse>(content);

            if (vesselFinderResponse?.Ais == null)
            {
                throw new Exception("Failed to deserialize vessel data or AIS data is null.");
            }

            var aisData = vesselFinderResponse.Ais;
            var vessel = new Vessel
            {
                IMO = aisData.Imo.ToString(),
                Mmsi = aisData.Mmsi.ToString(),
                Name = aisData.Name ?? "N/A",
                Positions = new List<VesselPosition>
                {
                    new VesselPosition
                    {
                        Latitude = aisData.Latitude,
                        Longitude = aisData.Longitude,
                        PositionTimestamp = DateTime.UtcNow,
                        SpeedOverGround = (decimal)aisData.Speed,
                        CourseOverGround = (decimal)aisData.Course
                    }
                },
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return vessel;
        }
    }
}