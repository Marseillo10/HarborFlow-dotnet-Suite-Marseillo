using System.Threading.Tasks;

namespace HarborFlowSuite.Application.Services
{
    public interface IServiceRequestNotifier
    {
        Task NotifyRequestUpdated();
    }
}
