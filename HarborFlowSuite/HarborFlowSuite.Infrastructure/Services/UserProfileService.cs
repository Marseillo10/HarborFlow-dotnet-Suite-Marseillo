using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Services;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly ApplicationDbContext _context;

        public UserProfileService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(string userId, string email)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.FirebaseUid == userId);

            if (user == null)
            {
                // User exists in Firebase but not in our DB, so create them.
                user = new Core.Models.User
                {
                    FirebaseUid = userId,
                    Email = email,
                    FullName = email, // Default FullName to email, can be updated later
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return new UserProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role?.Name ?? "Unknown"
            };
        }

        public async Task UpdateUserProfileAsync(string userId, UserProfileDto userProfileDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == userId);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            user.FullName = userProfileDto.FullName;
            user.Email = userProfileDto.Email;

            await _context.SaveChangesAsync();
        }
    }
}
