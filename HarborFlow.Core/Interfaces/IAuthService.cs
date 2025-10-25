using HarborFlow.Core.Models;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(string username, string password, string email, string fullName);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> UserExistsAsync(string username);
    }
}
