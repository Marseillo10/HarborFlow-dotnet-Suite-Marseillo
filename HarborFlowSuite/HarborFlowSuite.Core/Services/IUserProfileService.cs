using HarborFlowSuite.Core.DTOs;
using System.Threading.Tasks;

namespace HarborFlowSuite.Core.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId, string email);
        Task UpdateUserProfileAsync(string userId, UserProfileDto userProfileDto);
    }
}
