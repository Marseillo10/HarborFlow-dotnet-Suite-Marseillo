using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly HarborFlowDbContext _context;

        public AuthService(HarborFlowDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return user;
            }

            return null;
        }

        public async Task<User> RegisterAsync(string username, string password, string email, string fullName)
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

            return newUser;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
