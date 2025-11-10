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

namespace HarborFlowSuite.Server.Services
{
    public class AisDataService : BackgroundService
    {
        private readonly IHubContext<AisHub> _hubContext;
        private readonly IConfiguration _configuration;
        private ClientWebSocket _webSocket;
        private readonly string _apiKey;

        public AisDataService(IHubContext<AisHub> hubContext, IConfiguration configuration)
        {
            _hubContext = hubContext;
            _configuration = configuration;
            _webSocket = new ClientWebSocket();
            _apiKey = _configuration["AisStreamApiKey"];
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
                            BoundingBoxes = new[] { new[] { new[] { -90, -180 }, new[] { 90, 180 } } },
                            FilterMessageTypes = new[] { "PositionReport" }
                        };
                        var messageBuffer = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subscriptionMessage));
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
            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var aisMessage = JsonSerializer.Deserialize<AisMessage>(message);

                if (aisMessage?.MessageType == "PositionReport")
                {
                    var positionReport = aisMessage.Message.PositionReport;
                    var mmsi = positionReport.UserID.ToString();
                    var lat = positionReport.Latitude;
                    var lon = positionReport.Longitude;
                    var heading = positionReport.TrueHeading;
                    var speed = positionReport.Sog;
                    var name = $"Vessel {mmsi}";

                    await _hubContext.Clients.All.SendAsync("ReceiveVesselPositionUpdate", mmsi, lat, lon, heading, speed, name, stoppingToken);
                }
            }
        }
    }
}
