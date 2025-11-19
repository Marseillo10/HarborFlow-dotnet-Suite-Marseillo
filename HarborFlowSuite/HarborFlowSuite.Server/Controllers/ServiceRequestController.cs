using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;
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

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest([FromBody] ServiceRequest serviceRequest)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("User not found.");
            }
            var createdServiceRequest = await _serviceRequestService.CreateServiceRequest(serviceRequest, firebaseUid);

            if (createdServiceRequest == null)
            {
                return BadRequest("User not found or error creating service request.");
            }

            return CreatedAtAction(nameof(GetServiceRequests), new { id = createdServiceRequest.Id }, createdServiceRequest);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id, [FromBody] ApprovalDto approvalDto)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _serviceRequestService.ApproveServiceRequest(id, firebaseUid, approvalDto.Comments);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id, [FromBody] ApprovalDto approvalDto)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _serviceRequestService.RejectServiceRequest(id, firebaseUid, approvalDto.Comments);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}