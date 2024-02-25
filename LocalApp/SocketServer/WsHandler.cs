using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace LocalApp.SocketServer
{
    public class WsHandler
    {
        private readonly ConcurrentBag<WebSocket> Connections = new ConcurrentBag<WebSocket>();
        
        public Task AddSocket(WebSocket ws, ConnectionInfo connectionInfo)
        {
            Connections.Add(ws);

            return ReceiveMessage(ws);
        }

        public async Task ReceiveMessage(WebSocket ws)
        {
            var buffer = new byte[1024 * 4];
            var input = new ArraySegment<byte>(buffer);

            while (ws.State == WebSocketState.Open)
            {
                var ret = await ws.ReceiveAsync(input, CancellationToken.None);
                if (ret.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                {
                    Connections.TryTake(out _);

                    await ws.CloseAsync(ret.CloseStatus.Value, ret.CloseStatusDescription, CancellationToken.None);
                    return;
                }

                var msg = Encoding.UTF8.GetString(input);
                Console.WriteLine($"Received message: {msg}");

                var output = new ArraySegment<byte>(Encoding.UTF8.GetBytes("This is a msg from LocalApp use send method"));
                await ws.SendAsync(output, WebSocketMessageType.Text, true, CancellationToken.None);

            }

            Console.WriteLine($"Connection gone");
        }
    }
}
