using HarborFlow.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class CachingService : ICachingService
    {
        private readonly string _cachePath;
        private readonly ILogger<CachingService> _logger;

        public CachingService(ILogger<CachingService> logger)
        {
            _logger = logger;
            _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HarborFlow", "Cache");
            if (!Directory.Exists(_cachePath))
            {
                Directory.CreateDirectory(_cachePath);
            }
        }

        public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? absoluteExpiration = null)
        {
            var cacheFile = Path.Combine(_cachePath, key);
            if (File.Exists(cacheFile))
            {
                var lastWriteTime = File.GetLastWriteTimeUtc(cacheFile);
                if (!absoluteExpiration.HasValue || (DateTime.UtcNow - lastWriteTime) < absoluteExpiration.Value)
                {
                    try
                    {
                        var json = await File.ReadAllTextAsync(cacheFile);
                        var cachedItem = JsonSerializer.Deserialize<T>(json);
                        if (cachedItem != null) return cachedItem;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize cached item for key {Key}.", key);
                    }
                }
            }

            var item = await factory();
            try
            {
                var json = JsonSerializer.Serialize(item);
                await File.WriteAllTextAsync(cacheFile, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to serialize and cache item for key {Key}.", key);
            }

            return item;
        }

        public Task InvalidateAsync(string key)
        {
            var cacheFile = Path.Combine(_cachePath, key);
            if (File.Exists(cacheFile))
            {
                try
                {
                    File.Delete(cacheFile);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to invalidate cache for key {Key}.", key);
                }
            }
            return Task.CompletedTask;
        }
    }
}
