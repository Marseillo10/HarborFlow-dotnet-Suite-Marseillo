using HarborFlowSuite.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public class MockServiceRequestService : IServiceRequestService
    {
        private List<ServiceRequest> _serviceRequests;

        public MockServiceRequestService()
        {
            _serviceRequests = new List<ServiceRequest>
            {
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequesterId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Fuel Refueling Request",
                    Description = "Request for 5000 liters of marine diesel for Ever Given.",
                    Status = ServiceRequestStatus.Pending,
                    Priority = 1,
                    RequestedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequesterId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Cargo Inspection for Majestic Maersk",
                    Description = "Pre-loading inspection for hazardous materials.",
                    Status = ServiceRequestStatus.Approved,
                    Priority = 2,
                    RequestedAt = DateTime.UtcNow.AddDays(-5),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequesterId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Crew Change for MSC Gulsun",
                    Description = "Arrangement for 10 crew members to disembark and 12 to embark.",
                    Status = ServiceRequestStatus.InProgress,
                    Priority = 1,
                    RequestedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-12)
                },
                new ServiceRequest
                {
                    Id = Guid.NewGuid(),
                    RequesterId = Guid.NewGuid(),
                    CompanyId = Guid.NewGuid(),
                    Title = "Waste Disposal for HMM Algeciras",
                    Description = "Disposal of solid waste and bilge water.",
                    Status = ServiceRequestStatus.Rejected,
                    Priority = 3,
                    RequestedAt = DateTime.UtcNow.AddDays(-10),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    UpdatedAt = DateTime.UtcNow.AddDays(-8)
                }
            };
        }

        public Task<List<ServiceRequest>> GetServiceRequests()
        {
            return Task.FromResult(_serviceRequests);
        }

        public Task<ServiceRequest> GetServiceRequestById(Guid id)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.Id == id);
            return Task.FromResult(request);
        }

        public Task<ServiceRequest> CreateServiceRequest(ServiceRequest serviceRequest)
        {
            serviceRequest.Id = Guid.NewGuid();
            serviceRequest.CreatedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;
            _serviceRequests.Add(serviceRequest);
            return Task.FromResult(serviceRequest);
        }

        public Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            var existingRequest = _serviceRequests.FirstOrDefault(r => r.Id == serviceRequest.Id);
            if (existingRequest != null)
            {
                existingRequest.Title = serviceRequest.Title;
                existingRequest.Description = serviceRequest.Description;
                existingRequest.Status = serviceRequest.Status;
                existingRequest.Priority = serviceRequest.Priority;
                existingRequest.UpdatedAt = DateTime.UtcNow;
            }
            return Task.FromResult(existingRequest);
        }

        public Task<bool> DeleteServiceRequest(Guid id)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                _serviceRequests.Remove(request);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<ServiceRequest> ApproveServiceRequest(Guid id, Guid approverId, string comments)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                request.Status = ServiceRequestStatus.Approved;
                request.UpdatedAt = DateTime.UtcNow;
                // In a real scenario, you'd add to ApprovalHistory
            }
            return Task.FromResult(request);
        }

        public Task<ServiceRequest> RejectServiceRequest(Guid id, Guid approverId, string comments)
        {
            var request = _serviceRequests.FirstOrDefault(r => r.Id == id);
            if (request != null)
            {
                request.Status = ServiceRequestStatus.Rejected;
                request.UpdatedAt = DateTime.UtcNow;
                // In a real scenario, you'd add to ApprovalHistory
            }
            return Task.FromResult(request);
        }
    }
}
