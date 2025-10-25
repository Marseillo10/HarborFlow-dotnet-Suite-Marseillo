using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
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

        public async Task<Vessel> GetVesselDataAsync(string imo)
        {
            // The VesselFinder API has been removed as per user request as it is not a free service.
            // This method now returns a placeholder Vessel object.
            // Further integration with a different data provider is needed to restore full functionality.
            await Task.CompletedTask; // Represents a completed async operation.

            var vessel = new Vessel
            {
                IMO = imo,
                Mmsi = "N/A",
                Name = "N/A (Data Source Removed)",
                Positions = new List<VesselPosition>(), // No position data from this service
                UpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return vessel;
        }
    }
}