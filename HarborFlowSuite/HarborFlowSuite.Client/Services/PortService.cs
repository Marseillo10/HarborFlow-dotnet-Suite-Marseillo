using System.Net.Http.Json;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Client.Services
{
    public class PortService
    {
        private readonly HttpClient _httpClient;

        public PortService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Port>> GetPortsAsync(double? minLat = null, double? maxLat = null, double? minLon = null, double? maxLon = null)
        {
            try
            {
                var query = "api/Ports";
                var queryParams = new List<string>();

                if (minLat.HasValue) queryParams.Add($"minLat={minLat.Value}");
                if (maxLat.HasValue) queryParams.Add($"maxLat={maxLat.Value}");
                if (minLon.HasValue) queryParams.Add($"minLon={minLon.Value}");
                if (maxLon.HasValue) queryParams.Add($"maxLon={maxLon.Value}");

                if (queryParams.Any())
                {
                    query += "?" + string.Join("&", queryParams);
                }

                return await _httpClient.GetFromJsonAsync<List<Port>>(query) ?? new List<Port>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ports: {ex.Message}");
                return new List<Port>();
            }
        }
    }
}
