using System.Net.Http.Json;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;

namespace HarborFlowSuite.Client.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _httpClient;

        public CompanyService(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("HarborFlowSuite.ServerAPI");
        }

        public async Task<List<Company>> GetCompanies()
        {
            return await _httpClient.GetFromJsonAsync<List<Company>>("api/company");
        }

        public async Task<Company> GetCompany(Guid id)
        {
            return await _httpClient.GetFromJsonAsync<Company>($"api/company/{id}");
        }

        public async Task<Company> CreateCompany(CreateCompanyDto createCompanyDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/company", createCompanyDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Company>();
        }

        public async Task<bool> UpdateCompany(Guid id, Company company)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/company/{id}", company);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCompany(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"api/company/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
