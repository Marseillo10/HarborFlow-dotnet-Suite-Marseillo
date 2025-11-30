using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarborFlowSuite.Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using HarborFlowSuite.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HarborFlowSuite.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IHubContext<AisHub> _hubContext;

        public ServiceRequestController(IServiceRequestService serviceRequestService, IHubContext<AisHub> hubContext)
        {
            _serviceRequestService = serviceRequestService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<ServiceRequest>>> GetServiceRequests()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var serviceRequests = await _serviceRequestService.GetServiceRequests(firebaseUid);
            return Ok(serviceRequests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRequest>> GetServiceRequest(Guid id)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var serviceRequest = await _serviceRequestService.GetServiceRequestById(id, firebaseUid);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            return Ok(serviceRequest);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceRequest>> CreateServiceRequest([FromBody] CreateServiceRequestDto createServiceRequestDto)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(firebaseUid))
            {
                return Unauthorized("User not found.");
            }

            // Map DTO to Entity
            var serviceRequest = new ServiceRequest
            {
                Title = createServiceRequestDto.Title,
                Description = createServiceRequestDto.Description,
                Status = Enum.Parse<ServiceRequestStatus>(createServiceRequestDto.Status),
                Priority = createServiceRequestDto.Priority,
                VesselId = createServiceRequestDto.VesselId
            };

            var createdServiceRequest = await _serviceRequestService.CreateServiceRequest(serviceRequest, firebaseUid);

            if (createdServiceRequest == null)
            {
                return BadRequest("User not found or error creating service request.");
            }

            await _hubContext.Clients.All.SendAsync("ReceiveServiceRequestUpdate");

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
            await _hubContext.Clients.All.SendAsync("ReceiveServiceRequestUpdate");
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
            await _hubContext.Clients.All.SendAsync("ReceiveServiceRequestUpdate");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceRequest(Guid id, [FromBody] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return BadRequest();
            }

            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var updatedRequest = await _serviceRequestService.UpdateServiceRequest(serviceRequest, firebaseUid);
            if (updatedRequest == null)
            {
                return NotFound();
            }

            await _hubContext.Clients.All.SendAsync("ReceiveServiceRequestUpdate");

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceRequest(Guid id)
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _serviceRequestService.DeleteServiceRequest(id, firebaseUid);
            if (!result)
            {
                return NotFound();
            }
            await _hubContext.Clients.All.SendAsync("ReceiveServiceRequestUpdate");
            return NoContent();
        }
        [HttpGet("export")]
        public async Task<IActionResult> Export()
        {
            var firebaseUid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var serviceRequests = await _serviceRequestService.GetServiceRequests(firebaseUid);
            var builder = new System.Text.StringBuilder();
            builder.AppendLine("Id,Title,Description,Status,Priority,RequestedAt");

            foreach (var req in serviceRequests)
            {
                builder.AppendLine($"{req.Id},{EscapeCsv(req.Title)},{EscapeCsv(req.Description)},{req.Status},{req.Priority},{req.RequestedAt}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "service_requests.csv");
        }

        private string EscapeCsv(string field)
        {
            if (string.IsNullOrEmpty(field)) return "";
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
            {
                return $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
}