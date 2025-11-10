using HarborFlowSuite.Core.Models;
using System;
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

        public async Task<ServiceRequest> GetServiceRequestById(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<ServiceRequest>($"api/servicerequest/{id}");
        }

        public async Task<ServiceRequest> CreateServiceRequest(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/servicerequest", serviceRequest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }

        public async Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/servicerequest/{serviceRequest.Id}", serviceRequest);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }

        public async Task<bool> DeleteServiceRequest(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/servicerequest/{id}");
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<ServiceRequest> ApproveServiceRequest(Guid id, Guid approverId, string comments)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/servicerequest/{id}/approve", new { approverId, comments });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }

        public async Task<ServiceRequest> RejectServiceRequest(Guid id, Guid approverId, string comments)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/servicerequest/{id}/reject", new { approverId, comments });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequest>();
        }
    }
}
