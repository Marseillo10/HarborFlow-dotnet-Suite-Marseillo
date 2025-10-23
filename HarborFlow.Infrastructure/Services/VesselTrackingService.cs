
using HarborFlow.Application.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Infrastructure.DTOs.AisStream;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HarborFlow.Infrastructure.Services
{
    public class VesselTrackingService : IVesselTrackingService, IDisposable
    {
        private readonly HarborFlowDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<VesselTrackingService> _logger;
        private readonly SynchronizationContext? _syncContext;
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cancellationTokenSource;

        public ObservableCollection<Vessel> TrackedVessels { get; } = new ObservableCollection<Vessel>();

        public VesselTrackingService(HarborFlowDbContext context, IConfiguration configuration, ILogger<VesselTrackingService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _syncContext = SynchronizationContext.Current;
        }

        public async Task StartTracking(double[][] boundingBoxes)
        {
            var apiKey = _configuration["ApiKeys:AisStream"];
            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_API_KEY"))
            {
                _logger.LogWarning("AisStream API key is not configured. Cannot start tracking.");
                return;
            }

            if (_webSocket?.State == WebSocketState.Open) return;

            _webSocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            var uri = new Uri("wss://stream.aisstream.io/v0/stream");

            try
            {
                await _webSocket.ConnectAsync(uri, _cancellationTokenSource.Token);
                _logger.LogInformation("WebSocket connection established.");

                var subscriptionMessage = new
                {
                    APIKey = apiKey,
                    BoundingBoxes = new[] { boundingBoxes }
                };

                var messageBuffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(subscriptionMessage)));
                await _webSocket.SendAsync(messageBuffer, WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
                _logger.LogInformation("Subscription message sent.");

                _ = Task.Run(() => ListenForMessages(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start vessel tracking.");
            }
        }

        private async Task ListenForMessages(CancellationToken token)
        {
            var buffer = new byte[1024 * 4];
            while (_webSocket?.State == WebSocketState.Open && !token.IsCancellationRequested)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                        break;
                    }

                    var messageString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var aisMessage = JsonSerializer.Deserialize<AisStreamMessage>(messageString);

                    if (aisMessage?.MessageType == "PositionReport")
                    {
                        _syncContext?.Post(_ => 
                        {
                            var vessel = TrackedVessels.FirstOrDefault(v => v.Mmsi == aisMessage.MetaData.Mmsi.ToString());
                            if (vessel != null)
                            {
                                // Update existing vessel
                                vessel.Positions.Add(new VesselPosition
                                {
                                    Latitude = (decimal)aisMessage.Message.Latitude,
                                    Longitude = (decimal)aisMessage.Message.Longitude,
                                    PositionTimestamp = DateTime.UtcNow,
                                    SpeedOverGround = (decimal)aisMessage.Message.SpeedOverGround,
                                    CourseOverGround = (decimal)aisMessage.Message.CourseOverGround
                                });
                                vessel.UpdatedAt = DateTime.UtcNow;
                            }
                            else
                            {
                                // Add new vessel
                                var newVessel = new Vessel
                                {
                                    IMO = aisMessage.MetaData.Imo.ToString(),
                                    Mmsi = aisMessage.MetaData.Mmsi.ToString(),
                                    Name = aisMessage.MetaData.Name,
                                    VesselType = ConvertNavStatToVesselType(aisMessage.Message.NavigationalStatus),
                                    FlagState = aisMessage.MetaData.Flag,
                                    CreatedAt = DateTime.UtcNow,
                                    UpdatedAt = DateTime.UtcNow,
                                    Positions = new List<VesselPosition>
                                    {
                                        new VesselPosition
                                        {
                                            Latitude = (decimal)aisMessage.Message.Latitude,
                                            Longitude = (decimal)aisMessage.Message.Longitude,
                                            PositionTimestamp = DateTime.UtcNow,
                                            SpeedOverGround = (decimal)aisMessage.Message.SpeedOverGround,
                                            CourseOverGround = (decimal)aisMessage.Message.CourseOverGround
                                        }
                                    }
                                };
                                TrackedVessels.Add(newVessel);
                            }
                        }, null);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while listening for WebSocket messages.");
                }
            }
        }

        public async Task StopTracking()
        {
            if (_webSocket != null)
            {
                _cancellationTokenSource?.Cancel();
                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client stopping", CancellationToken.None);
                }
                _webSocket.Dispose();
                _webSocket = null;
                _logger.LogInformation("WebSocket connection closed.");
            }
        }

        private VesselType ConvertNavStatToVesselType(int navstat)
        {
            return navstat switch
            {
                7 => VesselType.Fishing,
                8 => VesselType.Sailing,
                _ => VesselType.Other, // Simplified mapping
            };
        }

        public async Task<Vessel?> GetVesselByImoAsync(string imo)
        {
            return await _context.Vessels.FirstOrDefaultAsync(v => v.IMO == imo);
        }

        public async Task<IEnumerable<Vessel>> SearchVesselsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<Vessel>();

            // This now searches the in-memory collection for simplicity with streaming data
            return await Task.FromResult(TrackedVessels
                .Where(v => v.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || v.IMO.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<IEnumerable<Vessel>> GetAllVesselsAsync()
        {
            return await _context.Vessels.ToListAsync();
        }

        public async Task<Vessel> AddVesselAsync(Vessel vessel)
        {
            if (vessel == null)
                throw new ArgumentNullException(nameof(vessel));

            // IMO is the primary key, so it must be provided.
            if (string.IsNullOrWhiteSpace(vessel.IMO))
                throw new ArgumentException("Vessel IMO cannot be empty.", nameof(vessel.IMO));

            var existingVessel = await _context.Vessels.FindAsync(vessel.IMO);
            if (existingVessel != null)
                throw new InvalidOperationException("A vessel with this IMO already exists.");

            vessel.CreatedAt = DateTime.UtcNow;
            vessel.UpdatedAt = DateTime.UtcNow;

            await _context.Vessels.AddAsync(vessel);
            await _context.SaveChangesAsync();
            return vessel;
        }

        public async Task<Vessel> UpdateVesselAsync(Vessel vessel)
        {
            if (vessel == null)
                throw new ArgumentNullException(nameof(vessel));

            var existingVessel = await _context.Vessels.FindAsync(vessel.IMO);
            if (existingVessel == null)
                throw new KeyNotFoundException("Vessel not found.");

            existingVessel.Name = vessel.Name;
            existingVessel.FlagState = vessel.FlagState;
            existingVessel.VesselType = vessel.VesselType;
            existingVessel.Mmsi = vessel.Mmsi;
            existingVessel.LengthOverall = vessel.LengthOverall;
            existingVessel.Beam = vessel.Beam;
            existingVessel.GrossTonnage = vessel.GrossTonnage;
            existingVessel.UpdatedAt = DateTime.UtcNow;
            
            // No need to call _context.Vessels.Update(existingVessel); EF Core tracks changes.
            await _context.SaveChangesAsync();
            return existingVessel;
        }

        public async Task DeleteVesselAsync(string imo)
        {
            var vessel = await _context.Vessels.FirstOrDefaultAsync(v => v.IMO == imo);
            if (vessel != null)
            {
                _context.Vessels.Remove(vessel);
                await _context.SaveChangesAsync();
            }
        }

        public void Dispose()
        {
            StopTracking().Wait();
            _cancellationTokenSource?.Dispose();
        }
    }
}
