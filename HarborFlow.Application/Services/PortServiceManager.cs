using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure;
using HarborFlow.Infrastructure.Services; // Assuming OfflineChange is here
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HarborFlow.Application.Services
{
    public class PortServiceManager : IPortServiceManager
    {
        private readonly HarborFlowDbContext _context;
        private readonly ILogger<PortServiceManager> _logger;
        private readonly ISynchronizationService _syncService;
        private bool _isOnline = true; // This should be managed by a network detection service

        public PortServiceManager(HarborFlowDbContext context, ILogger<PortServiceManager> logger, ISynchronizationService syncService)
        {
            _context = context;
            _logger = logger;
            _syncService = syncService;
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(Guid requestId)
        {
            try
            {
                return await _context.ServiceRequests.FindAsync(requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting service request {RequestId}.", requestId);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequest>> GetServiceRequestsByStatusAsync(RequestStatus status)
        {
            try
            {
                return await _context.ServiceRequests
                    .Where(r => r.Status == status)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting service requests by status {Status}.", status);
                throw;
            }
        }

        public async Task<ServiceRequest> SubmitServiceRequestAsync(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!_isOnline)
            {
                var change = new OfflineChange
                {
                    ChangeType = ChangeType.Add,
                    EntityType = nameof(ServiceRequest),
                    DataJson = JsonSerializer.Serialize(request)
                };
                await _syncService.AddChangeToQueueAsync(change);
                _logger.LogInformation("Service request {RequestId} queued for submission.", request.RequestId);
                return request; // Return the request as is, it will be processed later
            }

            try
            {
                request.RequestId = Guid.NewGuid();
                request.Status = RequestStatus.Submitted;
                request.CreatedAt = DateTime.UtcNow;
                request.UpdatedAt = DateTime.UtcNow;

                await _context.ServiceRequests.AddAsync(request);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service request {RequestId} submitted successfully.", request.RequestId);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while submitting service request for vessel {VesselImo}.", request.VesselImo);
                throw;
            }
        }

        public async Task<ServiceRequest> ApproveServiceRequestAsync(Guid requestId, Guid approverId)
        {
            try
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

                _logger.LogInformation("Service request {RequestId} approved successfully by {ApproverId}.", requestId, approverId);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while approving service request {RequestId}.", requestId);
                throw;
            }
        }

        public async Task<ServiceRequest> RejectServiceRequestAsync(Guid requestId, Guid rejectorId, string reason)
        {
            try
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

                _logger.LogInformation("Service request {RequestId} rejected successfully by {RejectorId}.", requestId, rejectorId);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while rejecting service request {RequestId}.", requestId);
                throw;
            }
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync(User currentUser)
        {
            if (currentUser == null) 
                return Enumerable.Empty<ServiceRequest>();

            try
            {
                IQueryable<ServiceRequest> query = _context.ServiceRequests;

                if (currentUser.Role == UserRole.MaritimeAgent || currentUser.Role == UserRole.VesselOperator)
                {
                    query = query.Where(r => r.RequestedBy == currentUser.UserId);
                }
                // Administrators and PortOfficers can see all requests, so no filter is applied

                return await query.OrderByDescending(r => r.RequestDate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all service requests for user {UserId}.", currentUser.UserId);
                throw;
            }
        }

        public async Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (!_isOnline)
            {
                var change = new OfflineChange
                {
                    ChangeType = ChangeType.Update,
                    EntityType = nameof(ServiceRequest),
                    DataJson = JsonSerializer.Serialize(request)
                };
                await _syncService.AddChangeToQueueAsync(change);
                _logger.LogInformation("Service request {RequestId} queued for update.", request.RequestId);
                return request;
            }

            try
            {
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
                _logger.LogInformation("Service request {RequestId} updated successfully.", request.RequestId);
                return existingRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating service request {RequestId}.", request.RequestId);
                throw;
            }
        }

        public async Task DeleteServiceRequestAsync(Guid requestId)
        {
            if (!_isOnline)
            {
                var change = new OfflineChange
                {
                    ChangeType = ChangeType.Delete,
                    EntityType = nameof(ServiceRequest),
                    DataJson = JsonSerializer.Serialize(new { RequestId = requestId })
                };
                await _syncService.AddChangeToQueueAsync(change);
                _logger.LogInformation("Service request {RequestId} queued for deletion.", requestId);
                return;
            }

            try
            {
                var request = await _context.ServiceRequests.FindAsync(requestId);
                if (request != null)
                {
                    _context.ServiceRequests.Remove(request);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Service request {RequestId} deleted successfully.", requestId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting service request {RequestId}.", requestId);
                throw;
            }
        }
    }
}
