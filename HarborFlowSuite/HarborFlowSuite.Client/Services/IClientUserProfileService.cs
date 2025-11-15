
using HarborFlowSuite.Core.DTOs;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IClientUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId, string email);
        Task UpdateUserProfileAsync(string userId, UserProfileDto userProfileDto);
    }
}
