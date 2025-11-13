using HarborFlowSuite.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HarborFlowSuite.Core.Services
{
    public interface IPortService
    {
        Task<IEnumerable<Port>> GetPortsAsync(IEnumerable<string> countries);
    }
}
