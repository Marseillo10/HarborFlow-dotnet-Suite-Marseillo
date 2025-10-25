using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly HarborFlowDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(HarborFlowDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    user.LastLogin = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User {Username} logged in successfully.", username);
                    return user;
                }

                _logger.LogWarning("Failed login attempt for user {Username}.", username);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for user {Username}.", username);
                throw;
            }
        }

        public async Task<User> RegisterAsync(string username, string password, string email, string fullName)
        {
            try
            {
                var newUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Username = username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                    Email = email,
                    FullName = fullName,
                    Role = UserRole.VesselOperator, // Default role for new registrations
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Username} registered successfully.", username);
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for user {Username}.", username);
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(User userUpdate)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userUpdate.UserId);
                if (user == null) return false;

                user.Username = userUpdate.Username;
                user.Email = userUpdate.Email;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User {UserId} updated successfully.", user.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}.", userUpdate.UserId);
                return false;
            }
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Password change failed for user {UserId}.", userId);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                user.UpdatedAt = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Password for user {UserId} changed successfully.", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}.", userId);
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user {Username} exists.", username);
                throw;
            }
        }
    }
}
