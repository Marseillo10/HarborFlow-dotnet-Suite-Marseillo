using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HarborFlow.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HarborFlow.Backend.Tests.Services
{
    public class CachingServiceTests : IDisposable
    {
        private readonly string _testCachePath;
        private readonly ILogger<CachingService> _logger;

        // A simple class for testing serialization
        private class TestCacheObject
        {
            public string Name { get; set; } = "";
            public int Value { get; set; }
        }

        public CachingServiceTests()
        {
            // Isolate tests in a dedicated sub-folder
            _testCachePath = Path.Combine(Path.GetTempPath(), "HarborFlowTestCache");
            if (Directory.Exists(_testCachePath))
            {
                Directory.Delete(_testCachePath, true);
            }
            Directory.CreateDirectory(_testCachePath);

            _logger = new Mock<ILogger<CachingService>>().Object;
        }

        // Custom CachingService that uses our test path
        private CachingService GetCachingService()
        {
            // This is a bit of a hack since the path is hardcoded. 
            // A better solution would be to inject the path, but for this test, we work with the existing code.
            // We can achieve this by changing the current directory for the duration of the test, 
            // but a cleaner way for this specific implementation is to create a derived class for testing.
            return new TestableCachingService(_logger, _testCachePath);
        }

        private class TestableCachingService : CachingService
        {
            // Override the private _cachePath by creating a new service that allows setting it.
            public TestableCachingService(ILogger<CachingService> logger, string cachePath) : base(logger)
            {
                // The base constructor already creates the default path, we just overwrite it here for our tests.
                // This is not ideal, but works for testing the current implementation without refactoring it.
                var cachePathField = typeof(CachingService).GetField("_cachePath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                cachePathField?.SetValue(this, cachePath);
            }
        }

        [Fact]
        public async Task GetOrSetAsync_WhenCacheMiss_ShouldExecuteFactoryAndWriteToFile()
        {
            // Arrange
            var service = GetCachingService();
            var key = "cache-miss-key";
            var factoryExecuted = false;
            Func<Task<TestCacheObject>> factory = () => 
            {
                factoryExecuted = true;
                return Task.FromResult(new TestCacheObject { Name = "Fresh Data", Value = 100 });
            };

            // Act
            var result = await service.GetOrSetAsync(key, factory);

            // Assert
            factoryExecuted.Should().BeTrue();
            result.Name.Should().Be("Fresh Data");
            
            var cacheFile = Path.Combine(_testCachePath, key);
            File.Exists(cacheFile).Should().BeTrue();
            var json = await File.ReadAllTextAsync(cacheFile);
            var fileContent = JsonSerializer.Deserialize<TestCacheObject>(json);
            fileContent.Should().BeEquivalentTo(result);
        }

        [Fact]
        public async Task GetOrSetAsync_WhenCacheHit_ShouldReturnCachedItemAndNotExecuteFactory()
        {
            // Arrange
            var service = GetCachingService();
            var key = "cache-hit-key";
            var cachedObject = new TestCacheObject { Name = "Cached Data", Value = 200 };
            var cacheFile = Path.Combine(_testCachePath, key);
            await File.WriteAllTextAsync(cacheFile, JsonSerializer.Serialize(cachedObject));

            var factoryExecuted = false;
            Func<Task<TestCacheObject>> factory = () =>
            {
                factoryExecuted = true;
                return Task.FromResult(new TestCacheObject { Name = "Should Not Be Called" });
            };

            // Act
            var result = await service.GetOrSetAsync(key, factory);

            // Assert
            factoryExecuted.Should().BeFalse();
            result.Should().BeEquivalentTo(cachedObject);
        }

        [Fact]
        public async Task GetOrSetAsync_WhenCacheIsExpired_ShouldExecuteFactoryAndOverwrite()
        {
            // Arrange
            var service = GetCachingService();
            var key = "expired-key";
            var expiredObject = new TestCacheObject { Name = "Expired Data", Value = 300 };
            var cacheFile = Path.Combine(_testCachePath, key);
            await File.WriteAllTextAsync(cacheFile, JsonSerializer.Serialize(expiredObject));
            File.SetLastWriteTimeUtc(cacheFile, DateTime.UtcNow.AddHours(-2)); // Set modification time to 2 hours ago

            var factoryExecuted = false;
            Func<Task<TestCacheObject>> factory = () =>
            {
                factoryExecuted = true;
                return Task.FromResult(new TestCacheObject { Name = "New Fresh Data" });
            };

            // Act
            var result = await service.GetOrSetAsync(key, factory, TimeSpan.FromHours(1));

            // Assert
            factoryExecuted.Should().BeTrue();
            result.Name.Should().Be("New Fresh Data");
        }

        [Fact]
        public async Task InvalidateAsync_ShouldDeleteCacheFile()
        {
            // Arrange
            var service = GetCachingService();
            var key = "invalidate-key";
            var cacheFile = Path.Combine(_testCachePath, key);
            await File.WriteAllTextAsync(cacheFile, "{}");

            // Act
            await service.InvalidateAsync(key);

            // Assert
            File.Exists(cacheFile).Should().BeFalse();
        }

        public void Dispose()
        {
            // Final cleanup after all tests in the class have run
            if (Directory.Exists(_testCachePath))
            {
                Directory.Delete(_testCachePath, true);
            }
        }
    }
}
