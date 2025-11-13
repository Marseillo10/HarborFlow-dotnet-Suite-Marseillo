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
        private List<Port>? _ports;

        public PortService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task LoadPortsAsync()
        {
            if (_ports == null)
            {
                _ports = await _httpClient.GetFromJsonAsync<List<Port>>("sample-data/ports.json");
            }
        }

        public async Task<IEnumerable<Port>> GetPortsAsync(IEnumerable<string> countries)
        {
            await LoadPortsAsync();

            if (countries == null || !countries.Any() || _ports == null)
            {
                return Enumerable.Empty<Port>();
            }

            var filteredPorts = _ports.Where(p => countries.Contains(p.Country, System.StringComparer.OrdinalIgnoreCase));
            return filteredPorts;
        }
    }
}
