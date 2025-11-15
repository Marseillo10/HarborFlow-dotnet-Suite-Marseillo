using HarborFlowSuite.Abstractions.Services;
using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userProfileService, ILogger<UserProfileController> logger)
        {
            _userProfileService = userProfileService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var userProfile = await _userProfileService.GetUserProfileAsync(userId, email);
            return Ok(userProfile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDto userProfileDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _userProfileService.UpdateUserProfileAsync(userId, userProfileDto);
            return NoContent();
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            try
            {
                var link = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email);
                _logger.LogInformation($"Password reset link for {email}: {link}");
                // In a real application, you would email this link to the user.
                return Ok();
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogError(ex, "Error generating password reset link");
                return StatusCode(500, "An unexpected error occurred while generating the password reset link.");
            }
        }
    }
}