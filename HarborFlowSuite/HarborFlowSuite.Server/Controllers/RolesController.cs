using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Shared.Constants;
using HarborFlowSuite.Application.Services;

namespace HarborFlowSuite.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;

    public RolesController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Policy = Permissions.Users.Manage)] // Only admins/managers should see roles
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _userService.GetAllRolesAsync();
        return Ok(roles);
    }
}
