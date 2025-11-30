using HarborFlowSuite.Core.DTOs;
using System.Net.Http.Json;

namespace HarborFlowSuite.Client.Services
{
    public class NewsService : INewsService
    {
        private readonly HttpClient _httpClient;

        public NewsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NewsItemDto>> GetNews()
        {
            return await _httpClient.GetFromJsonAsync<List<NewsItemDto>>("api/news") ?? new List<NewsItemDto>();
        }
    }
}
