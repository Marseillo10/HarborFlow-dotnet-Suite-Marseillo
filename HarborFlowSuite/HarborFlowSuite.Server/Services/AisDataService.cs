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

namespace HarborFlowSuite.Server.Services
{
    public class AisDataService : BackgroundService
    {
        private readonly IHubContext<AisHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private ClientWebSocket _webSocket;
        private readonly string _apiKey;
        private readonly ConcurrentDictionary<string, string> _vesselTypes = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _vesselNames = new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, DateTime> _lastGfwFetchTime = new ConcurrentDictionary<string, DateTime>();

        public AisDataService(IHubContext<AisHub> hubContext, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _webSocket = new ClientWebSocket();
            _apiKey = _configuration["AisStreamApiKey"] ?? string.Empty;
            if (string.IsNullOrEmpty(_apiKey))
            {
                Console.WriteLine("WARNING: AisStreamApiKey is missing in configuration. AIS data will not be fetched.");
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
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

                        var subscriptionMessage = new
                        {
                            APIkey = _apiKey,
                            BoundingBoxes = new[] { new[] { new[] { -13.1816069, 94.7717124 }, new[] { 6.92805288332, 151.7489081 } } },
                            FilterMessageTypes = new[] { "PositionReport", "ShipStaticData" }
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
            var buffer = new byte[1024 * 16]; // Increased buffer size to 16KB
            while (_webSocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                AisMessage? aisMessage = null;
                try
                {
                    aisMessage = JsonSerializer.Deserialize<AisMessage>(message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON Deserialization error: {ex.Message}");
                    continue; // Skip this message
                }

                if (aisMessage?.MessageType == "ShipStaticData")
                {
                    var staticData = aisMessage.Message.ShipStaticData;
                    var mmsi = staticData.UserID.ToString();
                    var vesselType = GetVesselType(staticData.Type);
                    _vesselTypes.AddOrUpdate(mmsi, vesselType, (key, oldValue) => vesselType);

                    if (!string.IsNullOrEmpty(staticData.Name))
                    {
                        var name = staticData.Name.Trim();
                        _vesselNames.AddOrUpdate(mmsi, name, (key, oldValue) => name);
                    }
                }

                if (aisMessage?.MessageType == "PositionReport")
                {
                    var positionReport = aisMessage.Message.PositionReport;
                    var mmsi = positionReport.UserID.ToString();
                    var lat = positionReport.Latitude;
                    var lon = positionReport.Longitude;
                    var heading = positionReport.TrueHeading;
                    var speed = positionReport.Sog;

                    var name = _vesselNames.GetValueOrDefault(mmsi, $"Vessel {mmsi}");
                    var vesselType = _vesselTypes.GetValueOrDefault(mmsi, "Other");

                    // Send position update immediately
                    await _hubContext.Clients.All.SendAsync("ReceiveVesselPositionUpdate", mmsi, lat, lon, heading, speed, name, vesselType, null, stoppingToken);

                    // Fetch metadata in the background
                    _ = Task.Run(async () =>
                    {
                        if (!_lastGfwFetchTime.TryGetValue(mmsi, out var lastFetch) || DateTime.UtcNow - lastFetch > TimeSpan.FromHours(1))
                        {
                            _lastGfwFetchTime[mmsi] = DateTime.UtcNow;
                            using (var scope = _serviceScopeFactory.CreateScope())
                            {
                                var gfwMetadataService = scope.ServiceProvider.GetRequiredService<IGfwMetadataService>();
                                var metadata = await gfwMetadataService.GetVesselMetadataAsync(mmsi);
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
                    }, stoppingToken);
                }
            }
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
    }
}
