using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Abstractions.Services;
using System.Threading.Tasks;

namespace HarborFlowSuite.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        public Task<UserProfileDto> GetUserProfileAsync(string userId, string email)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateUserProfileAsync(string userId, UserProfileDto userProfileDto)
        {
            throw new System.NotImplementedException();
        }
    }
}
