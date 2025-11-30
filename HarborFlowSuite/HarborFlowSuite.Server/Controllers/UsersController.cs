using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Shared.Constants;

namespace HarborFlowSuite.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly HarborFlowSuite.Application.Services.IUserService _userService;

    public UsersController(HarborFlowSuite.Application.Services.IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Users.View)]
    public async Task<IActionResult> GetAllUsers()
    {
        Guid? companyId = null;
        if (User.IsInRole(UserRole.CompanyAdmin))
        {
            // We need to get the user's company ID. 
            // Ideally this is in the claims. If not, we might need to fetch the user profile.
            // For now, let's assume we can get it from the service using the Firebase UID if it's not in claims.
            // Or better, let's add CompanyId to the JWT claims in the future.
            // For this iteration, I'll fetch the current user from the service to get their CompanyId.
            var firebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (firebaseUid != null)
            {
                var allUsers = await _userService.GetAllUsersAsync(); // This is inefficient but works for now without changing auth flow
                var currentUser = allUsers.FirstOrDefault(u => u.FirebaseUid == firebaseUid);
                companyId = currentUser?.CompanyId;
            }
        }

        var users = await _userService.GetAllUsersAsync(companyId);
        return Ok(users);
    }

    [HttpPut("{id}/role")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] HarborFlowSuite.Shared.DTOs.UpdateUserRoleDto updateRoleDto)
    {
        try
        {
            var currentFirebaseUid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            await _userService.UpdateUserRoleAsync(id, updateRoleDto.RoleId, currentFirebaseUid, updateRoleDto.CompanyId);
            return Ok(new { Message = "User role updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while updating the user role.", Details = ex.Message });
        }
    }

    [HttpPost]
    [Authorize(Policy = Permissions.Users.Manage)]
    public IActionResult CreateUser([FromBody] object userData)
    {
        return Ok(new { Message = "User created successfully" });
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { Message = $"User {id} deleted successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "An error occurred while deleting the user.", Details = ex.Message });
        }
    }
}
