#nullable enable
using Microsoft.Extensions.Hosting;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using HarborFlowSuite.Server.Hubs;
using System.Text.Json;
using HarborFlowSuite.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using HarborFlowSuite.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using HarborFlowSuite.Application.Services;
using System.Threading.Channels;

namespace HarborFlowSuite.Server.Services
{
    public class AisDataService : BackgroundService, IAisDataService
    {
        private readonly IHubContext<AisHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory; // Changed from _serviceScopeFactory to _scopeFactory
        private ClientWebSocket _webSocket;
        private readonly string _apiKey;
        private readonly ConcurrentDictionary<string, string> _vesselTypes = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _vesselNames = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, DateTime> _lastGfwFetchTime = new ConcurrentDictionary<string, DateTime>();
        private readonly ConcurrentDictionary<string, VesselPositionUpdateDto> _vesselPositions = new ConcurrentDictionary<string, VesselPositionUpdateDto>();
        private readonly Channel<string> _messageChannel;
        private Timer _cleanupTimer; // Changed to non-readonly
        private readonly IGfwApiService _gfwApiService; // New field
        private readonly ILogger<AisDataService> _logger; // New field
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(); // New field
        private readonly ConcurrentDictionary<string, (VesselPositionUpdateDto Info, VesselMetadataDto Metadata)> _vesselCache = new();
        private readonly ConcurrentDictionary<string, DateTime> _vesselFirstSeen = new(); // New field
        private readonly ConcurrentDictionary<string, bool> _gfwChecked = new(); // New field
        private Timer _gfwFetchTimer; // New field

        public IEnumerable<VesselPositionUpdateDto> GetActiveVessels()
        {
            // This method needs to be updated to use _vesselCache if it's still needed.
            // For now, returning an empty enumerable as _vesselPositions is removed.
            return Enumerable.Empty<VesselPositionUpdateDto>();
        }

        public AisDataService(
            IHubContext<AisHub> hubContext,
            IServiceScopeFactory scopeFactory, // Changed from serviceScopeFactory to scopeFactory
            IGfwApiService gfwApiService, // New parameter
            IConfiguration configuration,
            ILogger<AisDataService> logger) // New parameter
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _gfwApiService = gfwApiService;
            _configuration = configuration;
            _logger = logger;
            _webSocket = new ClientWebSocket();
            _apiKey = _configuration["AisStreamApiKey"] ?? string.Empty;

            // Unbounded channel for high throughput
            _messageChannel = Channel.CreateUnbounded<string>();

            // Cleanup timer: runs every 10 minutes
            _cleanupTimer = new Timer(CleanupStaleVessels, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
            _gfwFetchTimer = new Timer(CheckForMissingDataAndFetchGfw, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));

            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("WARNING: AisStreamApiKey is missing in configuration. AIS data will not be fetched.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Start the consumer loop
            _ = ProcessMessagesAsync(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_webSocket.State != WebSocketState.Open)
                    {
                        _webSocket = new ClientWebSocket();
                        var uri = new Uri("wss://stream.aisstream.io/v0/stream");
                        await _webSocket.ConnectAsync(uri, stoppingToken);
                        Console.WriteLine("Connected to AIS Stream.");

                        var boundingBox = _configuration.GetSection("AisStream:BoundingBox").Get<double[][][]>();
                        if (boundingBox == null)
                        {
                            // Default fallback if config is missing
                            boundingBox = new[] { new[] { new[] { -13.1816069, 94.7717124 }, new[] { 6.92805288332, 151.7489081 } } };
                        }

                        var subscriptionMessage = new
                        {
                            APIkey = _apiKey,
                            BoundingBoxes = boundingBox,
                            FilterMessageTypes = new[] { "PositionReport", "ShipStaticData", "StandardClassBPositionReport" }
                        };
                        var jsonMsg = JsonSerializer.Serialize(subscriptionMessage);
                        Console.WriteLine($"Sending subscription (Length: {jsonMsg.Length})");
                        var messageBuffer = Encoding.UTF8.GetBytes(jsonMsg);
                        await _webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, stoppingToken);
                        Console.WriteLine("Subscribed to AIS Stream.");
                    }

                    await ReceiveLoop(stoppingToken);
                }
                catch (WebSocketException ex)
                {
                    Console.WriteLine($"WebSocket error: {ex.Message}. Reconnecting in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error: {ex.Message}. Reconnecting in 5 seconds...");
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
            }
        }

        private async Task ReceiveLoop(CancellationToken stoppingToken)
        {
            var buffer = new byte[1024 * 16];
            while (_webSocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await _messageChannel.Writer.WriteAsync(message, stoppingToken);
            }
        }

        private async Task ProcessMessagesAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in _messageChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var aisMessage = JsonSerializer.Deserialize<AisMessage>(message);
                    if (aisMessage == null) continue;

                    if (aisMessage.MessageType == "ShipStaticData")
                    {
                        await ProcessStaticData(aisMessage.Message.ShipStaticData, stoppingToken);
                    }
                    else if (aisMessage.MessageType == "PositionReport")
                    {
                        await ProcessPositionReport(aisMessage.Message.PositionReport, stoppingToken);
                    }
                    else if (aisMessage.MessageType == "StandardClassBPositionReport")
                    {
                        await ProcessClassBPositionReport(aisMessage.Message.StandardClassBPositionReport, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            }
        }

        private async Task ProcessStaticData(AisMessage.ShipStaticDataMessage staticData, CancellationToken stoppingToken)
        {
            var mmsi = staticData.UserID.ToString();
            var vesselType = GetVesselType(staticData.Type);
            _vesselTypes.AddOrUpdate(mmsi, vesselType, (key, oldValue) => vesselType);

            if (!string.IsNullOrEmpty(staticData.Name))
            {
                var name = staticData.Name.Trim();
                _vesselNames.AddOrUpdate(mmsi, name, (key, oldValue) => name);
            }

            var metadata = new VesselMetadataDto
            {
                ImoNumber = staticData.ImoNumber > 0 ? staticData.ImoNumber.ToString() : string.Empty,
                ShipName = staticData.Name?.Trim() ?? string.Empty,
                Callsign = staticData.CallSign?.Trim() ?? string.Empty,
                Length = staticData.Dimension != null ? (double)(staticData.Dimension.A + staticData.Dimension.B) : null,
                Width = staticData.Dimension != null ? (double)(staticData.Dimension.C + staticData.Dimension.D) : null,
                Destination = staticData.Destination?.Trim() ?? string.Empty,
                Eta = ParseEta(staticData.Eta),
                Draught = staticData.MaximumStaticDraught
            };

            // Update existing cache entry or create a new one with metadata
            _vesselCache.AddOrUpdate(mmsi,
                (new VesselPositionUpdateDto { MMSI = mmsi, VesselType = vesselType, Name = metadata.ShipName, Metadata = metadata }, metadata),
                (key, old) => (old.Info, metadata));

            await _hubContext.Clients.All.SendAsync("ReceiveVesselMetadataUpdate", mmsi, metadata, stoppingToken);
        }

        private async Task ProcessPositionReport(AisMessage.PositionReportMessage positionReport, CancellationToken stoppingToken)
        {
            var mmsi = positionReport.UserID.ToString();
            var name = _vesselNames.GetValueOrDefault(mmsi, $"MMSI: {mmsi}");
            var vesselType = _vesselTypes.GetValueOrDefault(mmsi, "Other");

            var updateDto = new VesselPositionUpdateDto
            {
                MMSI = mmsi,
                Latitude = positionReport.Latitude,
                Longitude = positionReport.Longitude,
                Heading = positionReport.TrueHeading,
                Speed = positionReport.Sog,
                Name = name,
                VesselType = vesselType,
                NavigationalStatus = GetNavigationalStatus(positionReport.NavigationalStatus),
                RecordedAt = DateTime.UtcNow
            };

            await BroadcastPositionUpdate(updateDto, stoppingToken);
        }

        private async Task ProcessClassBPositionReport(AisMessage.StandardClassBPositionReportMessage report, CancellationToken stoppingToken)
        {
            var mmsi = report.UserID.ToString();
            var name = _vesselNames.GetValueOrDefault(mmsi, $"MMSI: {mmsi}");
            var vesselType = _vesselTypes.GetValueOrDefault(mmsi, "Other"); // Class B usually doesn't send type in position report

            var updateDto = new VesselPositionUpdateDto
            {
                MMSI = mmsi,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                Heading = report.TrueHeading,
                Speed = report.Sog,
                Name = name,
                VesselType = vesselType,
                NavigationalStatus = "Underway (Class B)", // Class B often implies underway/active
                RecordedAt = DateTime.UtcNow
            };

            await BroadcastPositionUpdate(updateDto, stoppingToken);
        }

        private async Task BroadcastPositionUpdate(VesselPositionUpdateDto updateDto, CancellationToken stoppingToken)
        {
            // Try to find the vessel ID in the database (cached or scoped lookup)
            Guid? vesselId = null;
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HarborFlowSuite.Infrastructure.Persistence.ApplicationDbContext>();
                var vessel = await dbContext.Vessels.IgnoreQueryFilters().FirstOrDefaultAsync(v => v.MMSI == updateDto.MMSI, stoppingToken);
                if (vessel != null)
                {
                    vesselId = vessel.Id;
                }
            }
            updateDto.VesselId = vesselId;

            _vesselPositions.AddOrUpdate(updateDto.MMSI, updateDto, (key, oldValue) => updateDto);

            // Update cache
            _vesselCache.AddOrUpdate(updateDto.MMSI,
                (updateDto, new VesselMetadataDto()),
                (key, old) => (updateDto, old.Metadata));

            // Track first seen
            _vesselFirstSeen.TryAdd(updateDto.MMSI, DateTime.UtcNow);

            await _hubContext.Clients.All.SendAsync("ReceiveVesselPositionUpdate", updateDto, stoppingToken);

            // Trigger metadata fetch if needed
            _ = FetchGfwMetadataIfNeeded(updateDto.MMSI, stoppingToken);
        }

        private async Task FetchGfwMetadataIfNeeded(string mmsi, CancellationToken stoppingToken)
        {
            if (!_lastGfwFetchTime.TryGetValue(mmsi, out var lastFetch) || DateTime.UtcNow - lastFetch > TimeSpan.FromHours(1))
            {
                _lastGfwFetchTime[mmsi] = DateTime.UtcNow;
                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        // Use the injected service instead of resolving from scope if possible, or resolve correctly
                        // Since _gfwApiService is singleton/scoped and injected, we can use it directly if it's thread safe.
                        // However, if we need a fresh scope for EF Core (if GfwApiService uses it), we should resolve it.
                        // GfwApiService uses HttpClient, so it should be fine.
                        // But wait, the original code tried to resolve IGfwMetadataService.
                        // We will use _gfwApiService.
                        var metadata = await _gfwApiService.GetVesselIdentityAsync(mmsi);
                        if (metadata != null)
                        {
                            if (!string.IsNullOrEmpty(metadata.ShipName))
                            {
                                _vesselNames.AddOrUpdate(mmsi, metadata.ShipName, (key, oldValue) => metadata.ShipName);
                            }
                            await _hubContext.Clients.All.SendAsync("ReceiveVesselMetadataUpdate", mmsi, metadata, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching GFW metadata for {mmsi}: {ex.Message}");
                }
            }
        }

        private void CleanupStaleVessels(object? state)
        {
            var threshold = DateTime.UtcNow.AddHours(-1);
            var staleKeys = _vesselPositions.Where(kvp => kvp.Value.RecordedAt < threshold).Select(kvp => kvp.Key).ToList();

            foreach (var key in staleKeys)
            {
                _vesselPositions.TryRemove(key, out _);
                _vesselCache.TryRemove(key, out _);
                _vesselFirstSeen.TryRemove(key, out _);
                _gfwChecked.TryRemove(key, out _);
            }

            if (staleKeys.Any())
            {
                Console.WriteLine($"Cleaned up {staleKeys.Count} stale vessels.");
            }
        }

        private DateTime? ParseEta(AisMessage.ShipStaticDataMessage.EtaData eta)
        {
            if (eta == null || eta.Month == 0 || eta.Day == 0) return null;
            try
            {
                var now = DateTime.UtcNow;
                var year = now.Year;
                // Simple logic: if month is earlier than current month, assume next year
                if (eta.Month < now.Month) year++;

                return new DateTime(year, eta.Month, eta.Day, eta.Hour, eta.Minute, 0);
            }
            catch
            {
                return null;
            }
        }

        private string GetNavigationalStatus(int status)
        {
            return status switch
            {
                0 => "Underway using engine",
                1 => "At anchor",
                2 => "Not under command",
                3 => "Restricted manoeuvrability",
                4 => "Constrained by her draught",
                5 => "Moored",
                6 => "Aground",
                7 => "Engaged in Fishing",
                8 => "Under way sailing",
                _ => "Unknown"
            };
        }

        private string GetVesselType(int typeCode)
        {
            if (typeCode >= 20 && typeCode <= 29) return "WIG";
            if (typeCode >= 30 && typeCode <= 39) return "Fishing";
            if (typeCode >= 40 && typeCode <= 49) return "HSC";
            if (typeCode >= 50 && typeCode <= 59) return "Other";
            if (typeCode >= 60 && typeCode <= 69) return "Passenger";
            if (typeCode >= 70 && typeCode <= 79) return "Cargo";
            if (typeCode >= 80 && typeCode <= 89) return "Tanker";
            if (typeCode >= 90 && typeCode <= 99) return "Other";
            return "Other";
        }
        private async void CheckForMissingDataAndFetchGfw(object? state)
        {
            try
            {
                var now = DateTime.UtcNow;
                var candidates = _vesselCache.Keys.ToList();

                foreach (var mmsi in candidates)
                {
                    if (!_vesselCache.TryGetValue(mmsi, out var data)) continue;
                    if (!_vesselFirstSeen.TryGetValue(mmsi, out var firstSeen)) continue;

                    // 1. Check if vessel has been seen for > 2 minutes
                    if ((now - firstSeen).TotalMinutes < 2) continue;

                    // 2. Check if we already checked GFW
                    if (_gfwChecked.ContainsKey(mmsi)) continue;

                    // 3. Check if data is missing (Name is MMSI/Empty OR Type is Other/Unknown OR Dimensions missing)
                    bool isNameMissing = string.IsNullOrEmpty(data.Info.Name) || data.Info.Name == mmsi;
                    bool isTypeUnknown = string.IsNullOrEmpty(data.Info.VesselType) || data.Info.VesselType.Contains("Other", StringComparison.OrdinalIgnoreCase) || data.Info.VesselType == "0";
                    bool isDimensionsMissing = !data.Info.Metadata?.Length.HasValue ?? true; // Width is not in PositionUpdateDto directly, usually in Metadata

                    if (isNameMissing || isTypeUnknown || isDimensionsMissing)
                    {
                        // Mark as checked immediately to prevent duplicate calls
                        _gfwChecked.TryAdd(mmsi, true);

                        _logger.LogInformation("Fetching GFW data for MMSI {MMSI} (Missing data detected)", mmsi);

                        var gfwMetadata = await _gfwApiService.GetVesselIdentityAsync(mmsi);
                        if (gfwMetadata != null)
                        {
                            // Merge GFW data
                            var mergedMetadata = new VesselMetadataDto
                            {
                                ImoNumber = !string.IsNullOrEmpty(gfwMetadata.ImoNumber) ? gfwMetadata.ImoNumber : data.Metadata?.ImoNumber,
                                ShipName = !string.IsNullOrEmpty(gfwMetadata.ShipName) ? gfwMetadata.ShipName : data.Metadata?.ShipName,
                                Callsign = !string.IsNullOrEmpty(gfwMetadata.Callsign) ? gfwMetadata.Callsign : data.Metadata?.Callsign,
                                Flag = !string.IsNullOrEmpty(gfwMetadata.Flag) ? gfwMetadata.Flag : data.Metadata?.Flag,
                                Length = gfwMetadata.Length > 0 ? gfwMetadata.Length : data.Metadata?.Length,
                                // GFW doesn't typically provide Width directly in search, but we might get it later
                                Destination = data.Metadata?.Destination, // GFW doesn't have this
                                Eta = data.Metadata?.Eta, // GFW doesn't have this
                                Draught = data.Metadata?.Draught,
                                Width = data.Metadata?.Width
                            };

                            // Update Cache
                            var updatedInfo = data.Info;
                            if (!string.IsNullOrEmpty(gfwMetadata.ShipName)) updatedInfo.Name = gfwMetadata.ShipName;
                            if (!string.IsNullOrEmpty(gfwMetadata.VesselType)) updatedInfo.VesselType = gfwMetadata.VesselType; // Update Type if available
                            // Update other fields in Info if needed
                            updatedInfo.Metadata = mergedMetadata;

                            _vesselCache[mmsi] = (updatedInfo, mergedMetadata);

                            // Broadcast update
                            await _hubContext.Clients.All.SendAsync("ReceiveVesselMetadataUpdate", mmsi, mergedMetadata);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GFW fetch loop");
            }
        }
    }
}
