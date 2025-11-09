using Microsoft.AspNetCore.SignalR;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarborFlowSuite.Server.Hubs
{
    public class AisHub : Hub
    {
        private ClientWebSocket? _webSocket;

        public override async Task OnConnectedAsync()
        {
            _webSocket = new ClientWebSocket();
            await _webSocket.ConnectAsync(new System.Uri("wss://stream.aisstream.io/v0/stream"), CancellationToken.None);

            var subscriptionMessage = new
            {
                APIkey = "6e4e5f4fc6ffdf9c3038f42de65c005a9a55a763",
                BoundingBoxes = new[]
                {
                    new[] { new[] { -90.0, -180.0 }, new[] { 90.0, 180.0 } }
                }
            };

            var messageBuffer = Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(subscriptionMessage));
            await _webSocket.SendAsync(new System.ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

            await base.OnConnectedAsync();

            _ = ReceiveLoop();
        }

        private async Task ReceiveLoop()
        {
            if (_webSocket == null)
            {
                return;
            }

            var buffer = new byte[1024 * 4];
            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new System.ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await Clients.All.SendAsync("ReceiveAisData", message);
                }
            }
        }

        public override async Task OnDisconnectedAsync(System.Exception? exception)
        {
            if (_webSocket != null)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                _webSocket.Dispose();
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
