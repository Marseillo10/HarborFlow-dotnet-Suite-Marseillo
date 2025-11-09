using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Client.Services
{
    public interface IServiceRequestService
    {
        Task<List<ServiceRequest>> GetServiceRequests();
    }
}
