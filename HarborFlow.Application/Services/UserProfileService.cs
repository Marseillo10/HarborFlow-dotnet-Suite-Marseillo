using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Application.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly HarborFlowDbContext _context;
        private readonly ILogger<UserProfileService> _logger;

        public UserProfileService(HarborFlowDbContext context, ILogger<UserProfileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User> GetUserProfileAsync(Guid userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting user profile for user {UserId}.", userId);
                throw;
            }
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(user.UserId);
                if (existingUser == null) return false;

                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("User profile for user {UserId} updated successfully.", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating user profile for user {UserId}.", user.UserId);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null || !BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Password change failed for user {UserId}.", userId);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Password for user {UserId} changed successfully.", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing password for user {UserId}.", userId);
                return false;
            }
        }
    }
}
