using HarborFlow.Core.Models;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface IUserProfileService
    {
        Task<User?> GetUserProfileAsync(Guid userId);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
