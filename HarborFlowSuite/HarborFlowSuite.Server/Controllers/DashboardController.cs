using Microsoft.AspNetCore.Mvc;
using HarborFlowSuite.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using HarborFlowSuite.Application.Services;

namespace HarborFlowSuite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("servicerequeststatus")]
        public async Task<ActionResult<IEnumerable<ServiceRequestStatusSummaryDto>>> GetServiceRequestStatusSummary()
        {
            var summary = await _dashboardService.GetServiceRequestStatusSummary();
            return Ok(summary);
        }

        [HttpGet("vesseltypes")]
        public async Task<ActionResult<IEnumerable<VesselTypeSummaryDto>>> GetVesselTypeSummary()
        {
            var summary = await _dashboardService.GetVesselTypeSummary();
            return Ok(summary);
        }
    }
}
