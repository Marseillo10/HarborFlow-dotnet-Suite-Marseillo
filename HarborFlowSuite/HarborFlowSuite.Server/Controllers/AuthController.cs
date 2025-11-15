using HarborFlowSuite.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Models;
using FirebaseAdmin.Auth;

namespace HarborFlowSuite.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
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
            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);

            // Save user to local database
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirebaseUid = userRecord.Uid,
                Email = userRecord.Email,
                FullName = userRecord.DisplayName,
                Role = null // Default role
            };
            var createdUser = await _authService.RegisterUserAsync(newUser);

            return Ok(createdUser);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogError(ex, "Error registering user with Firebase");
            if (ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists)
            {
                return BadRequest(new { message = "The email address is already in use by another account." });
            }
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during registration");
            return StatusCode(500, new { message = "An unexpected error occurred" });
        }
    }
}
