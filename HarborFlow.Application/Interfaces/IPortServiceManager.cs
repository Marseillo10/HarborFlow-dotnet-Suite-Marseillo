
using HarborFlow.Core.Models;

namespace HarborFlow.Application.Interfaces
{
    public interface IPortServiceManager
    {
        Task<ServiceRequest> SubmitServiceRequestAsync(ServiceRequest request);
        Task<ServiceRequest> ApproveServiceRequestAsync(Guid requestId, Guid approverId);
        Task<ServiceRequest> RejectServiceRequestAsync(Guid requestId, Guid rejectorId, string reason);
        Task<IEnumerable<ServiceRequest>> GetServiceRequestsByStatusAsync(RequestStatus status);
        Task<ServiceRequest?> GetServiceRequestByIdAsync(Guid requestId);
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync(User currentUser);
        Task<ServiceRequest> UpdateServiceRequestAsync(ServiceRequest request);
        Task DeleteServiceRequestAsync(Guid requestId);
    }
}
