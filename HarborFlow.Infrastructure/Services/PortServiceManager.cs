
using HarborFlow.Core.Models;
using HarborFlow.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HarborFlow.Infrastructure.Services
{
    public class PortServiceManager : IPortServiceManager
    {
        private readonly HarborFlowDbContext _context;

        public PortServiceManager(HarborFlowDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(Guid requestId)
        {
            return await _context.ServiceRequests.FindAsync(requestId);
        }

        public async Task<IEnumerable<ServiceRequest>> GetServiceRequestsByStatusAsync(RequestStatus status)
        {
            return await _context.ServiceRequests
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<ServiceRequest> SubmitServiceRequestAsync(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            request.RequestId = Guid.NewGuid();
            request.Status = RequestStatus.Submitted;
            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _context.ServiceRequests.AddAsync(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<ServiceRequest> ApproveServiceRequestAsync(Guid requestId, Guid approverId)
        {
            var request = await GetServiceRequestByIdAsync(requestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found.");

            request.Status = RequestStatus.Approved;
            request.UpdatedAt = DateTime.UtcNow;

            var history = new ApprovalHistory
            {
                ApprovalId = Guid.NewGuid(),
                RequestId = requestId,
                ApprovedBy = approverId,
                Action = ApprovalAction.Approve,
                ActionDate = DateTime.UtcNow
            };

            await _context.ApprovalHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<ServiceRequest> RejectServiceRequestAsync(Guid requestId, Guid rejectorId, string reason)
        {
            var request = await GetServiceRequestByIdAsync(requestId);
            if (request == null)
                throw new KeyNotFoundException("Service request not found.");

            request.Status = RequestStatus.Rejected;
            request.UpdatedAt = DateTime.UtcNow;
            request.Notes = reason;

            var history = new ApprovalHistory
            {
                ApprovalId = Guid.NewGuid(),
                RequestId = requestId,
                ApprovedBy = rejectorId,
                Action = ApprovalAction.Reject,
                ActionDate = DateTime.UtcNow,
                Reason = reason
            };

            await _context.ApprovalHistories.AddAsync(history);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync(User currentUser)
        {
            if (currentUser == null) 
                return Enumerable.Empty<ServiceRequest>();

            IQueryable<ServiceRequest> query = _context.ServiceRequests;

            if (currentUser.Role == UserRole.MaritimeAgent || currentUser.Role == UserRole.VesselOperator)
            {
                query = query.Where(r => r.RequestedBy == currentUser.UserId);
            }
            // Administrators and PortOfficers can see all requests, so no filter is applied

            return await query.OrderByDescending(r => r.RequestDate).ToListAsync();
        }

        public async Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existingRequest = await _context.ServiceRequests.FindAsync(request.RequestId);
            if (existingRequest == null)
                throw new KeyNotFoundException("Service request not found.");

            existingRequest.VesselImo = request.VesselImo;
            existingRequest.ServiceType = request.ServiceType;
            existingRequest.Description = request.Description;
            existingRequest.RequestDate = request.RequestDate;
            existingRequest.Status = request.Status;
            existingRequest.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingRequest;
        }
    }
}
