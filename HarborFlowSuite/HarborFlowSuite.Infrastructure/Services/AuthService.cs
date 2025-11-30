using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Core.DTOs;
using FirebaseAdmin.Auth;
using HarborFlowSuite.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        UserRecord userRecord;
        try
        {
            // Create user in Firebase
            var userArgs = new UserRecordArgs
            {
                Email = registerUserDto.Email,
                Password = registerUserDto.Password,
                DisplayName = registerUserDto.Name,
                EmailVerified = false,
                Disabled = false
            };
            userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
        }
        catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
        {
            // User already exists in Firebase, try to retrieve them
            userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(registerUserDto.Email);
        }

        // Check if user exists in local database
        var existingUser = _context.Users.FirstOrDefault(u => u.FirebaseUid == userRecord.Uid);
        if (existingUser != null)
        {
            // User already exists locally, throw exception to prevent re-registration
            throw new InvalidOperationException("The user with the provided email already exists (EMAIL_EXISTS).");
        }

        // Set default role
        string roleName = UserRole.Guest;

        // Auto-assign System Admin for UGM emails
        if (registerUserDto.Email.EndsWith("@mail.ugm.ac.id", StringComparison.OrdinalIgnoreCase))
        {
            roleName = UserRole.SystemAdmin;
        }

        // Retrieve role entity
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            // Create role if it doesn't exist
            role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = roleName == UserRole.SystemAdmin ? "System Administrator" : "Default Guest Role",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        // Set custom user claims in Firebase
        var claims = new Dictionary<string, object>
        {
            { "role", role.Name }
        };
        await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userRecord.Uid, claims);

        // Save user to local database
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirebaseUid = userRecord.Uid,
            Email = userRecord.Email,
            FullName = userRecord.DisplayName,
            RoleId = role.Id
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
}
