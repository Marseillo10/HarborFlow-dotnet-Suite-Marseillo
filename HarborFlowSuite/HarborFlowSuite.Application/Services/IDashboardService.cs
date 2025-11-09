using HarborFlowSuite.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Application.Services
{
    public interface IDashboardService
    {
        Task<IEnumerable<ServiceRequestStatusSummaryDto>> GetServiceRequestStatusSummary();
        Task<IEnumerable<VesselTypeSummaryDto>> GetVesselTypeSummary();
    }
}
