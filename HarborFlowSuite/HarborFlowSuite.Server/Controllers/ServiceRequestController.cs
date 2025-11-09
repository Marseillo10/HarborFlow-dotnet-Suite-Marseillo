using HarborFlowSuite.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceRequestController : ControllerBase
    {
        [HttpGet]
        public Task<ActionResult<List<ServiceRequest>>> GetServiceRequests()
        {
            var serviceRequests = new List<ServiceRequest>
            {
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorUserId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Fuel Refueling Request",
                    Description = "Request for 5000 liters of marine diesel for Ever Given.",
                    Status = ServiceRequestStatus.Pending.ToString(),
                    Priority = "High",
                    RequestedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorUserId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Cargo Inspection for Majestic Maersk",
                    Description = "Pre-loading inspection for hazardous materials.",
                    Status = ServiceRequestStatus.Approved.ToString(),
                    Priority = "Medium",
                    RequestedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorUserId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Crew Change for MSC Gulsun",
                    Description = "Arrangement for 10 crew members to disembark and 12 to embark.",
                    Status = ServiceRequestStatus.InProgress.ToString(),
                    Priority = "High",
                    RequestedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequestorUserId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Waste Disposal for HMM Algeciras",
                    Description = "Disposal of solid waste and bilge water.",
                    Status = ServiceRequestStatus.Rejected.ToString(),
                    Priority = "Low",
                    RequestedAt = DateTime.UtcNow.AddDays(-10),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-8)
                }
            };
            return Task.FromResult<ActionResult<List<ServiceRequest>>>(Ok(serviceRequests));
        }
    }
}