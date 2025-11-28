using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HarborFlowSuite.Infrastructure.Services;

public class PortService : IPortService
{
    private readonly ApplicationDbContext _context;

    public PortService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Port>> GetPorts()
    {
        return await _context.Ports.ToListAsync();
    }
}
