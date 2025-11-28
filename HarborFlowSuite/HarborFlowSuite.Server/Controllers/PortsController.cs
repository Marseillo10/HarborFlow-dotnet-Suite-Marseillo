using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HarborFlowSuite.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortsController : ControllerBase
{
    private readonly IPortService _portService;

    public PortsController(IPortService portService)
    {
        _portService = portService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Port>>> GetPorts()
    {
        var ports = await _portService.GetPorts();
        return Ok(ports);
    }
}
