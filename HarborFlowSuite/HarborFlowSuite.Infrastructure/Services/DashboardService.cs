using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.DTOs;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequestStatusSummaryDto>> GetServiceRequestStatusSummary()
        {
            return await _context.ServiceRequests
                .GroupBy(sr => sr.Status)
                .Select(g => new ServiceRequestStatusSummaryDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<VesselTypeSummaryDto>> GetVesselTypeSummary()
        {
            return await _context.Vessels
                .GroupBy(v => v.VesselType)
                .Select(g => new VesselTypeSummaryDto
                {
                    VesselType = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }
    }
}
