using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ServiceRequest>> GetServiceRequests()
        {
            return await _httpClient.GetFromJsonAsync<List<ServiceRequest>>("api/servicerequest");
        }
    }
}
