using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Shared.DTOs;
using HarborFlowSuite.Shared.Constants;
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

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(Guid? companyId = null)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Company)
                .AsQueryable();

            if (companyId.HasValue)
            {
                query = query.Where(u => u.CompanyId == companyId.Value);
            }

            return await query
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirebaseUid = u.FirebaseUid,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = u.Role != null ? u.Role.Name : "No Role",
                    RoleId = u.RoleId,
                    CompanyName = u.Company != null ? u.Company.Name : "No Company",
                    CompanyId = u.CompanyId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                })
                .ToListAsync();

            // Filter out duplicates by Name to ensure UI doesn't show multiple entries for the same role
            return roles.DistinctBy(r => r.Name).ToList();
        }

        public async Task UpdateUserRoleAsync(Guid userId, Guid roleId, string currentFirebaseUid, Guid? companyId = null)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }

            // Self-Protection: Prevent user from modifying their own role
            if (user.FirebaseUid == currentFirebaseUid)
            {
                throw new InvalidOperationException("You cannot modify your own role.");
            }

            var currentUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.FirebaseUid == currentFirebaseUid);

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("Current user not found.");
            }

            // Company Admin Restrictions
            if (currentUser.Role.Name == UserRole.CompanyAdmin)
            {
                if (currentUser.CompanyId == null)
                {
                    throw new InvalidOperationException("You are a Company Admin but have no company assigned. Please contact System Administrator.");
                }

                // Ensure target user belongs to the same company (unless we are assigning them to it, but here we are updating)
                // If target user has no company, maybe we are assigning them?
                if (user.CompanyId != null && user.CompanyId != currentUser.CompanyId)
                {
                    throw new UnauthorizedAccessException("You can only manage users within your company.");
                }

                // If attempting to change company, ensure it matches currentUser's company
                if (companyId.HasValue && companyId.Value != currentUser.CompanyId)
                {
                    throw new UnauthorizedAccessException("You cannot assign users to a different company.");
                }
            }

            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                throw new KeyNotFoundException($"Role with ID {roleId} not found.");
            }

            user.RoleId = roleId;

            // Only update company if a value is provided. 
            // Note: This prevents setting company to null (removing company). 
            // If that is needed, we need a more explicit signal.
            if (companyId.HasValue)
            {
                user.CompanyId = companyId.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Sync role change to Firebase Custom Claims
            try
            {
                var claims = new Dictionary<string, object>
                {
                    { "role", role.Name }
                };
                // If we updated the company, maybe we should add it to claims too?
                // For now, just role.
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
