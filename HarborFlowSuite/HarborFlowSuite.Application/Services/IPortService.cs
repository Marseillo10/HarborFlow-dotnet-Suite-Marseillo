using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Application.Services;

public interface IPortService
{
    Task<IEnumerable<Port>> GetPorts();
}
