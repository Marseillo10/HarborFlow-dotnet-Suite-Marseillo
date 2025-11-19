using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PortsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Port>>> GetPorts()
    {
        return await _context.Ports.ToListAsync();
    }
}
