using HarborFlow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly ILogger<SynchronizationService> _logger;

        public SynchronizationService(ILogger<SynchronizationService> logger)
        {
            _logger = logger;
        }

        public Task SynchronizeAsync()
        {
            _logger.LogInformation("Synchronization service is not yet implemented.");
            return Task.CompletedTask;
        }
    }
}
