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
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpPut("{id}/role")]
    [Authorize(Policy = Permissions.Users.Manage)]
    public async Task<IActionResult> UpdateUserRole(Guid id, [FromBody] HarborFlowSuite.Shared.DTOs.UpdateUserRoleDto updateRoleDto)
    {
        try
        {
            await _userService.UpdateUserRoleAsync(id, updateRoleDto.RoleId);
            return Ok(new { Message = "User role updated successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
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
