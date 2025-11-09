using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HarborFlowSuite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceRequest>>> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestService.GetServiceRequests();
            return Ok(serviceRequests);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalDto approvalDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _serviceRequestService.ApproveServiceRequest(id, userId, approvalDto.Comments);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] ApprovalDto approvalDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _serviceRequestService.RejectServiceRequest(id, userId, approvalDto.Comments);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }

    public class ApprovalDto
    {
        public string Comments { get; set; }
    }
}