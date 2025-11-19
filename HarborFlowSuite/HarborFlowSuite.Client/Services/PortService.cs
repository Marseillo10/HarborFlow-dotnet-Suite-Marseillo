using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class PortService : IPortService
    {
        private readonly HttpClient _httpClient;

        public PortService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HarborFlowSuite.ServerAPI");
        }

        public async Task<IEnumerable<Port>> GetPortsAsync(IEnumerable<string> countries)
        {
            try
            {
                var ports = await _httpClient.GetFromJsonAsync<List<Port>>("api/ports");

                if (ports == null)
                {
                    return Enumerable.Empty<Port>();
                }

                if (countries != null && countries.Any())
                {
                    return ports.Where(p => countries.Contains(p.Country, StringComparer.OrdinalIgnoreCase));
                }

                return ports;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching ports: {ex.Message}");
                return Enumerable.Empty<Port>();
            }
        }
    }
}
