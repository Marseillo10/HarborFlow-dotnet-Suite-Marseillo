using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Company)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirebaseUid = u.FirebaseUid,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role != null ? u.Role.Name : "Unknown",
                    RoleId = u.RoleId,
                    CompanyName = u.Company != null ? u.Company.Name : "N/A",
                    CompanyId = u.CompanyId,
                    IsActive = u.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();
        }

        public async Task UpdateUserRoleAsync(Guid userId, Guid roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            user.RoleId = roleId;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Sync role change to Firebase Custom Claims
            try
            {
                var claims = new Dictionary<string, object>
                {
                    { "role", role.Name }
                };
                await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(user.FirebaseUid, claims);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request? Or fail?
                // For now, let's log to console (should use ILogger in real app)
                Console.WriteLine($"[UserService] Failed to update Firebase claims for user {user.FirebaseUid}: {ex.Message}");
                // We might want to re-throw if consistency is critical, but for now let's allow DB update to persist.
            }
        }


        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // 1. Delete from Firebase Authentication
            if (!string.IsNullOrEmpty(user.FirebaseUid))
            {
                try
                {
                    await FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance.DeleteUserAsync(user.FirebaseUid);
                }
                catch (FirebaseAdmin.FirebaseException ex)
                {
                    // Log error, but proceed to delete from DB if it's a "user not found" error or similar?
                    // For now, we'll log and rethrow if it's critical, or just log and continue if we want to ensure DB cleanup.
                    // Let's log and continue to ensure local DB is cleaned up even if Firebase fails (e.g. user already deleted).
                    Console.WriteLine($"[UserService] Error deleting user {user.FirebaseUid} from Firebase: {ex.Message}");
                }
            }

            // 2. Delete from Database
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserByFirebaseIdAsync(string firebaseUid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Firebase UID {firebaseUid} not found.");
            }

            // Reuse the existing deletion logic by calling DeleteUserAsync with the internal ID
            // Or just duplicate the logic if we want to avoid double-lookup (though FindAsync is fast/cached)
            // Let's just call DeleteUserAsync to keep logic in one place.
            await DeleteUserAsync(user.Id);
        }
    }
}
