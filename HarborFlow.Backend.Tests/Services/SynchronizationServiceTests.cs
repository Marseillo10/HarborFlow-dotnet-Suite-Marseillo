using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace HarborFlow.Backend.Tests.Services
{
    public class SynchronizationServiceTests : IDisposable
    {
        private readonly string _testOfflinePath;
        private readonly string _testQueueFilePath;
        private readonly ILogger<SynchronizationService> _logger;

        public SynchronizationServiceTests()
        {
            // Isolate tests in a dedicated sub-folder
            _testOfflinePath = Path.Combine(Path.GetTempPath(), "HarborFlowTestOffline");
            if (Directory.Exists(_testOfflinePath))
            {
                Directory.Delete(_testOfflinePath, true);
            }
            Directory.CreateDirectory(_testOfflinePath);

            _testQueueFilePath = Path.Combine(_testOfflinePath, "OfflineQueue.json");
            _logger = new Mock<ILogger<SynchronizationService>>().Object;
        }

        // Custom SynchronizationService that uses our test path
        private SynchronizationService GetSynchronizationService()
        {
            return new TestableSynchronizationService(_logger, _testOfflinePath);
        }

        private class TestableSynchronizationService : SynchronizationService
        {
            public TestableSynchronizationService(ILogger<SynchronizationService> logger, string offlinePath) : base(logger)
            {
                // Override the private _queueFilePath field using reflection
                var fieldInfo = typeof(SynchronizationService).GetField("_queueFilePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fieldInfo?.SetValue(this, Path.Combine(offlinePath, "OfflineQueue.json"));

                // Manually call LoadQueueAsync again after path override
                var loadMethod = typeof(SynchronizationService).GetMethod("LoadQueueAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (loadMethod == null)
                {
                    throw new InvalidOperationException("Could not find private method 'LoadQueueAsync' via reflection.");
                }

                var loadTask = (Task?)loadMethod.Invoke(this, null);
                loadTask?.GetAwaiter().GetResult();
            }
        }

        [Fact]
        public async Task AddChangeToQueueAsync_ShouldWriteChangeToFile()
        {
            // Arrange
            var service = GetSynchronizationService();
            var change = new OfflineChange { ChangeType = ChangeType.Add, EntityType = "Vessel", DataJson = "{}" };

            // Act
            await service.AddChangeToQueueAsync(change);

            // Assert
            File.Exists(_testQueueFilePath).Should().BeTrue();
            var json = await File.ReadAllTextAsync(_testQueueFilePath);
            var queueFromFile = JsonSerializer.Deserialize<List<OfflineChange>>(json);
            queueFromFile.Should().HaveCount(1);
            queueFromFile[0].EntityType.Should().Be("Vessel");
        }

        [Fact]
        public async Task LoadQueueAsync_WhenFileExists_ShouldLoadQueueFromDisk()
        {
            // Arrange
            var initialQueue = new List<OfflineChange>
            {
                new OfflineChange { ChangeType = ChangeType.Update, EntityType = "ServiceRequest", DataJson = "{}" }
            };
            await File.WriteAllTextAsync(_testQueueFilePath, JsonSerializer.Serialize(initialQueue));

            // Act
            var service = GetSynchronizationService(); // The constructor loads the queue
            await service.AddChangeToQueueAsync(new OfflineChange { ChangeType = ChangeType.Delete, EntityType = "Port", DataJson = "{}" });

            // Assert
            var json = await File.ReadAllTextAsync(_testQueueFilePath);
            var queueFromFile = JsonSerializer.Deserialize<List<OfflineChange>>(json);
            queueFromFile.Should().HaveCount(2);
            queueFromFile[0].ChangeType.Should().Be(ChangeType.Update);
        }

        [Fact]
        public async Task SynchronizeAsync_ShouldProcessAndClearQueue()
        {
            // Arrange
            var service = GetSynchronizationService();
            await service.AddChangeToQueueAsync(new OfflineChange { ChangeType = ChangeType.Add, EntityType = "Vessel", DataJson = "{}" });

            // Act
            await service.SynchronizeAsync();

            // Assert
            var json = await File.ReadAllTextAsync(_testQueueFilePath);
            var queueFromFile = JsonSerializer.Deserialize<List<OfflineChange>>(json);
            queueFromFile.Should().BeEmpty();
        }

        public void Dispose()
        {
            // Final cleanup
            if (Directory.Exists(_testOfflinePath))
            {
                Directory.Delete(_testOfflinePath, true);
            }
        }
    }
}
