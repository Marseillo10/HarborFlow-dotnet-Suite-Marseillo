using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequest> ApproveServiceRequest(Guid id, Guid approverId, string comments)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return null;
            }

            serviceRequest.Status = ServiceRequestStatus.Approved;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            var approvalHistory = new ApprovalHistory
            {
                ServiceRequestId = id,
                ApproverId = approverId,
                Action = "Approved",
                Comments = comments,
                ActionAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApprovalHistories.Add(approvalHistory);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<ServiceRequest> CreateServiceRequest(ServiceRequest serviceRequest, string firebaseUid)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null)
            {
                // Or throw an exception, depending on how you want to handle this case
                return null;
            }

            serviceRequest.Id = Guid.NewGuid();
            serviceRequest.RequesterId = user.Id;
            serviceRequest.CompanyId = user.CompanyId;
            serviceRequest.CreatedAt = DateTime.UtcNow;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();
            return serviceRequest;
        }

        public async Task<bool> DeleteServiceRequest(Guid id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return false;
            }

            _context.ServiceRequests.Remove(serviceRequest);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ServiceRequest> GetServiceRequestById(Guid id)
        {
            return await _context.ServiceRequests.FindAsync(id);
        }

        public async Task<List<ServiceRequest>> GetServiceRequests()
        {
            return await _context.ServiceRequests.ToListAsync();
        }

        public async Task<ServiceRequest> RejectServiceRequest(Guid id, Guid approverId, string comments)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return null;
            }

            serviceRequest.Status = ServiceRequestStatus.Rejected;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            var approvalHistory = new ApprovalHistory
            {
                ServiceRequestId = id,
                ApproverId = approverId,
                Action = "Rejected",
                Comments = comments,
                ActionAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApprovalHistories.Add(approvalHistory);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest)
        {
            serviceRequest.UpdatedAt = DateTime.UtcNow;
            _context.Entry(serviceRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return serviceRequest;
        }
    }
}
