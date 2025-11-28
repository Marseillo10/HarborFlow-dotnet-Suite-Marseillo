using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Core.DTOs;
using FirebaseAdmin.Auth;

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

        // Save user to local database
        var newUser = new User
        {
            Id = Guid.NewGuid(),
            FirebaseUid = userRecord.Uid,
            Email = userRecord.Email,
            FullName = userRecord.DisplayName,
            Role = null // Default role
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
}
