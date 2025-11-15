using HarborFlowSuite.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Abstractions.Services
{
    public interface IDashboardService
    {
        Task<IEnumerable<ServiceRequestStatusSummaryDto>> GetServiceRequestStatusSummary();
        Task<IEnumerable<VesselTypeSummaryDto>> GetVesselTypeSummary();
    }
}
