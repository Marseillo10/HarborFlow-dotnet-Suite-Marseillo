using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Abstractions.Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class UserProfileService : IClientUserProfileService
    {
        private readonly HttpClient _httpClient;

        public UserProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(string userId, string email)
        {
            return await _httpClient.GetFromJsonAsync<UserProfileDto>("api/userprofile");
        }

        public async Task UpdateUserProfileAsync(string userId, UserProfileDto userProfileDto)
        {
            await _httpClient.PutAsJsonAsync("api/userprofile", userProfileDto);
        }
    }
}
