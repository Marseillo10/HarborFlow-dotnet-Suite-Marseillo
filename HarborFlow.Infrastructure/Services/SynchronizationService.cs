using HarborFlow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly string _queueFilePath;
        private readonly ILogger<SynchronizationService> _logger;
        private List<OfflineChange> _offlineQueue = new();

        public SynchronizationService(ILogger<SynchronizationService> logger)
        {
            _logger = logger;
            var offlineStoragePath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "HarborFlow", "Offline");
            if (!Directory.Exists(offlineStoragePath))
            {
                Directory.CreateDirectory(offlineStoragePath);
            }
            _queueFilePath = Path.Combine(offlineStoragePath, "OfflineQueue.json");
            LoadQueueAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async Task AddChangeToQueueAsync(object change)
        {
            if (change is not OfflineChange offlineChange) return;
            _offlineQueue.Add(offlineChange);
            await SaveQueueAsync();
        }

        public async Task SynchronizeAsync()
        {
            _logger.LogInformation("Starting synchronization...");
            // In a real implementation, you would process the queue here.
            // For now, we just log the pending changes.
            foreach (var change in _offlineQueue)
            {
                _logger.LogInformation($"Pending change: {change.ChangeType} on {change.EntityType} with data {change.DataJson}");
            }

            // Clear the queue after processing
            _offlineQueue.Clear();
            await SaveQueueAsync();
            _logger.LogInformation("Synchronization complete.");
        }

        private async Task LoadQueueAsync()
        {
            if (!File.Exists(_queueFilePath))
            {
                _offlineQueue = new List<OfflineChange>();
                return;
            }

            try
            {
                var json = await File.ReadAllTextAsync(_queueFilePath);
                _offlineQueue = JsonSerializer.Deserialize<List<OfflineChange>>(json) ?? new List<OfflineChange>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to load offline queue.");
                _offlineQueue = new List<OfflineChange>();
            }
        }

        private async Task SaveQueueAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(_offlineQueue);
                await File.WriteAllTextAsync(_queueFilePath, json);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to save offline queue.");
            }
        }
    }

    public class OfflineChange
    {
        public ChangeType ChangeType { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string DataJson { get; set; } = string.Empty;
    }

    public enum ChangeType
    {
        Add,
        Update,
        Delete
    }
}
