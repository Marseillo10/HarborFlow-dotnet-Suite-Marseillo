using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetServiceRequests();
        Task<ServiceRequest> GetServiceRequestById(Guid id);
        Task<ServiceRequest> CreateServiceRequest(ServiceRequest serviceRequest);
        Task<ServiceRequest> UpdateServiceRequest(ServiceRequest serviceRequest);
        Task<bool> DeleteServiceRequest(Guid id);
        Task<ServiceRequest> ApproveServiceRequest(Guid id, string comments);
        Task<ServiceRequest> RejectServiceRequest(Guid id, string comments);
    }
}
