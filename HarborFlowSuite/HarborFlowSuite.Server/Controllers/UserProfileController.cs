using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly HarborFlowSuite.Application.Services.IUserService _userService;

        public UserProfileController(IUserProfileService userProfileService, HarborFlowSuite.Application.Services.IUserService userService)
        {
            _userProfileService = userProfileService;
            _userService = userService;
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

        [HttpDelete]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                await _userService.DeleteUserByFirebaseIdAsync(userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while deleting the account.", Details = ex.Message });
            }
        }
    }
}
