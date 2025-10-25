using System.Threading.Tasks;

namespace HarborFlow.Core.Interfaces
{
    public interface ISynchronizationService
    {
        Task AddChangeToQueueAsync(object change);
        Task SynchronizeAsync();
    }
}
