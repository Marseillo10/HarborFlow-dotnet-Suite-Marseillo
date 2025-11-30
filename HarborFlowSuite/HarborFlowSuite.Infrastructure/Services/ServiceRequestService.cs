using HarborFlowSuite.Application.Services;
using HarborFlowSuite.Core.Models;
using HarborFlowSuite.Infrastructure.Persistence;
using HarborFlowSuite.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Infrastructure.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceRequestNotifier _notifier;

        public ServiceRequestService(ApplicationDbContext context, IServiceRequestNotifier notifier)
        {
            _context = context;
            _notifier = notifier;
        }

        public async Task<ServiceRequest> ApproveServiceRequest(Guid id, string firebaseUid, string comments)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return null;
            }

            serviceRequest.Status = ServiceRequestStatus.Approved;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null) return null;

            var approvalHistory = new ApprovalHistory
            {
                ServiceRequestId = id,
                ApproverId = user.Id,
                Action = "Approved",
                Comments = comments,
                ActionAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApprovalHistories.Add(approvalHistory);
            await _context.SaveChangesAsync();
            await _notifier.NotifyRequestUpdated();

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
            serviceRequest.RequestedAt = DateTime.UtcNow;

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            // Record creation history
            var history = new ApprovalHistory
            {
                ServiceRequestId = serviceRequest.Id,
                ApproverId = user.Id,
                Action = "Created",
                Comments = "Request created",
                ActionAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            _context.ApprovalHistories.Add(history);
            await _context.SaveChangesAsync();

            await _notifier.NotifyRequestUpdated();
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
            await _notifier.NotifyRequestUpdated();
            return true;
        }

        public async Task<ServiceRequest> GetServiceRequestById(Guid id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Requester)
                .Include(sr => sr.Vessel)
                .Include(sr => sr.Company)
                .Include(sr => sr.AssignedOfficer)
                .Include(sr => sr.ApprovalHistories)
                    .ThenInclude(ah => ah.Approver)
                .FirstOrDefaultAsync(sr => sr.Id == id);
        }

        public async Task<List<ServiceRequest>> GetServiceRequests(string firebaseUid)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

                if (user == null) return new List<ServiceRequest>();

                var query = _context.ServiceRequests
                    .Include(sr => sr.Requester)
                    .Include(sr => sr.Vessel)
                    .Include(sr => sr.Company)
                    .Include(sr => sr.AssignedOfficer)
                    .AsQueryable();

                if (user.Role?.Name == UserRole.VesselAgent)
                {
                    if (user.CompanyId.HasValue)
                    {
                        query = query.Where(sr => sr.CompanyId == user.CompanyId.Value);
                    }
                    else
                    {
                        // Vessel Agent without company sees nothing? Or maybe just their own requests?
                        // Requirement says "filter by company". If no company, safe to return empty or just own requests.
                        // Let's assume strict company filtering.
                        return new List<ServiceRequest>();
                    }
                }
                else if (user.Role?.Name == UserRole.Guest)
                {
                    return new List<ServiceRequest>();
                }
                // System Admin and Port Authority Officer see all (no filter added)

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetServiceRequests: {ex.Message}");
                throw;
            }
        }

        public async Task<ServiceRequest> RejectServiceRequest(Guid id, string firebaseUid, string comments)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                return null;
            }

            serviceRequest.Status = ServiceRequestStatus.Rejected;
            serviceRequest.UpdatedAt = DateTime.UtcNow;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user == null) return null;

            var approvalHistory = new ApprovalHistory
            {
                ServiceRequestId = id,
                ApproverId = user.Id,
                Action = "Rejected",
                Comments = comments,
                ActionAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.ApprovalHistories.Add(approvalHistory);
            await _context.SaveChangesAsync();
            await _notifier.NotifyRequestUpdated();

            return serviceRequest;
        }

        public async Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest, string firebaseUid)
        {
            var existingRequest = await _context.ServiceRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == serviceRequest.Id);
            if (existingRequest == null) return null;

            string GetPriorityLabel(int p) => p switch { 1 => "High", 2 => "Medium", 3 => "Low", _ => "Unknown" };

            var changes = new List<string>();
            if (existingRequest.Title != serviceRequest.Title) changes.Add($"Title: '{existingRequest.Title}' -> '{serviceRequest.Title}'");
            if (existingRequest.Description != serviceRequest.Description) changes.Add($"Description: '{existingRequest.Description}' -> '{serviceRequest.Description}'");
            if (existingRequest.Status != serviceRequest.Status) changes.Add($"Status: {existingRequest.Status} -> {serviceRequest.Status}");
            if (existingRequest.Priority != serviceRequest.Priority) changes.Add($"Priority: {GetPriorityLabel(existingRequest.Priority)} -> {GetPriorityLabel(serviceRequest.Priority)}");

            string changeLog = changes.Any() ? string.Join(", ", changes) : "Request details updated";

            serviceRequest.UpdatedAt = DateTime.UtcNow;
            _context.Entry(serviceRequest).State = EntityState.Modified;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);
            if (user != null)
            {
                var history = new ApprovalHistory
                {
                    ServiceRequestId = serviceRequest.Id,
                    ApproverId = user.Id,
                    Action = "Edited",
                    Comments = changeLog,
                    ActionAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                _context.ApprovalHistories.Add(history);
            }

            await _context.SaveChangesAsync();
            await _notifier.NotifyRequestUpdated();
            return serviceRequest;
        }
    }
}
