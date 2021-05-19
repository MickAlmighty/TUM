using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets
{
    public static class WebSocketClient
    {
        public static async Task<WebSocketConnection> ConnectAsync(Uri peer)
        {
            ClientWebSocket clientWebSocket = new ClientWebSocket();
            await clientWebSocket.ConnectAsync(peer, CancellationToken.None);
            switch (clientWebSocket.State)
            {
                case WebSocketState.Open:
                    Debug.WriteLine($"Opening client web socket connection to {peer}...");
                    WebSocketConnection webSocketConnection = new ClientConnection(clientWebSocket, peer.ToString());
                    return webSocketConnection;
                default:
                    throw new WebSocketException($"Invalid client web socket status: {clientWebSocket.State}.");
            }
        }
    }
}
