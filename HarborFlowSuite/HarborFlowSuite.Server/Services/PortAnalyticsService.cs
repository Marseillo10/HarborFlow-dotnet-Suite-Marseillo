using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HarborFlowSuite.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Core.Models;
using System.Collections.Concurrent;
using HarborFlowSuite.Shared.DTOs;
using HarborFlowSuite.Application.Services;

namespace HarborFlowSuite.Server.Services
{
    public class PortAnalyticsService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAisDataService _aisDataService;
        private readonly ILogger<PortAnalyticsService> _logger;
        private readonly TimeSpan _updateInterval = TimeSpan.FromMinutes(5);

        // In-memory cache for port congestion levels: PortId -> VesselCount
        // This could be moved to a shared cache or DB if scaling is needed.
        public static ConcurrentDictionary<Guid, int> PortCongestionLevels { get; } = new();

        public PortAnalyticsService(
            IServiceProvider serviceProvider,
            IAisDataService aisDataService,
            ILogger<PortAnalyticsService> logger)
        {
            _serviceProvider = serviceProvider;
            _aisDataService = aisDataService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("PortAnalyticsService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CalculatePortCongestionAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error calculating port congestion.");
                }

                await Task.Delay(_updateInterval, stoppingToken);
            }
        }

        private async Task CalculatePortCongestionAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Calculating port congestion...");

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Fetch all ports (or cache them if list is static)
                var ports = await dbContext.Ports.ToListAsync(stoppingToken);

                // Get all active vessels from AIS service
                // Note: We need a way to get ALL vessels. Currently GetActiveVessels returns a dictionary.
                // Assuming GetActiveVessels() is available on the interface or concrete class.
                // If not, we might need to expose it.
                // Let's assume we can get the concurrent dictionary or a snapshot.

                // Since IAisDataService might not expose the dictionary directly, let's cast or assume we can get it.
                // Ideally, IAisDataService should have a method `IEnumerable<VesselPositionDto> GetAllVessels()`.
                // For now, let's assume we can access the public property if we cast to concrete service or if interface has it.
                // Checking AisDataService implementation... it has `GetActiveVessels()`.

                var activeVessels = _aisDataService.GetActiveVessels();

                foreach (var port in ports)
                {
                    // Simple density calculation: Count vessels within X km radius
                    // Let's say 10km (~0.1 degrees roughly, but let's use Haversine for better accuracy or simple box for speed)
                    // Simple box is faster: +/- 0.1 degree is roughly 11km.

                    double searchRadiusDeg = 0.1;

                    int count = activeVessels.Count(v =>
                        Math.Abs(v.Latitude - port.Latitude) < searchRadiusDeg &&
                        Math.Abs(v.Longitude - port.Longitude) < searchRadiusDeg
                    );

                    PortCongestionLevels.AddOrUpdate(port.Id, count, (key, oldValue) => count);
                }
            }

            _logger.LogInformation("Port congestion calculation completed.");
        }
    }
}
