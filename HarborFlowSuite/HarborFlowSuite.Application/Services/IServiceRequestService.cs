using HarborFlowSuite.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Application.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetServiceRequests();
        Task<ServiceRequest> GetServiceRequestById(Guid id);
        Task<ServiceRequest> CreateServiceRequest(ServiceRequest serviceRequest, string firebaseUid);
        Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest);
        Task<bool> DeleteServiceRequest(Guid id);
        Task<ServiceRequest> ApproveServiceRequest(Guid id, Guid approverId, string comments);
        Task<ServiceRequest> RejectServiceRequest(Guid id, Guid approverId, string comments);
    }
}
