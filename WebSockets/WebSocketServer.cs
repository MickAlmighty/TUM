using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using Exception = System.Exception;

namespace WebSockets
{
    // Reference: https://github.com/mpostol/NBlockchain/blob/master/P2PPrototocol/NodeJSAPI/WebSocketServer.cs
    public static class WebSocketServer
    {
        public static ServerWebSocketConnection CreateServer(int p2p_port )
        {
            Uri uri = new Uri($@"http://localhost:{p2p_port}/");
            return new ServerConnection(uri);
        }

        private sealed class ServerConnection : ServerWebSocketConnection
        {
            private HashSet<WebSocketConnection> ClientConnections { get; } = new HashSet<WebSocketConnection>();

            private CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

            private Uri Uri { get; }
            private bool ServerStarted { get; set; }
            private ManualResetEvent ServerStopEvent { get; } = new ManualResetEvent(false);

            public ServerConnection(Uri uri)
            {
                Uri = uri;
            }

            public override async Task SendAsync(string message)
            {
                foreach (WebSocketConnection client in ClientConnections.ToList())
                {
                    await client.SendAsync(message);
                }
            }

            public override async Task DisconnectAsync()
            {
                TokenSource.Cancel();
                if (!ServerStarted || ServerStopEvent.WaitOne(0))
                {
                    return;
                }
                ServerStopEvent.WaitOne();
                foreach(WebSocketConnection conn in ClientConnections)
                {
                    try
                    {
                        await conn.DisconnectAsync();
                    }
                    catch (Exception e)
                    {
                        InvokeOnError(new OnErrorEventHandlerArgs(this, e));
                    }
                }
                ClientConnections.Clear();
            }

            protected override void Dispose(bool disposing)
            {
                DisconnectAsync().Wait();
            }

            public override async Task RunServerLoop()
            {
                if (ServerStarted)
                {
                    return;
                }
                ServerStarted = true;
                using (HttpListener listener = new HttpListener())
                {
                    listener.Prefixes.Add(Uri.ToString());
                    listener.Start();
                    while (!TokenSource.IsCancellationRequested)
                    {
                        HttpListenerContext listenerContext = await listener.GetContextAsync();
                        if (!listenerContext.Request.IsWebSocketRequest)
                        {
                            listenerContext.Response.StatusCode = 400;
                            listenerContext.Response.Close();
                        }
                        else
                        {
                            HttpListenerWebSocketContext webSocketContext =
                                await listenerContext.AcceptWebSocketAsync(null);
                            WebSocketConnection ws = new ClientConnection(webSocketContext.WebSocket,
                                listenerContext.Request.RemoteEndPoint?.ToString());
                            ClientConnections.Add(ws);
                            ws.OnMessage += (e, a) => InvokeOnMessage(a);
                            ws.OnError += (e, a) => InvokeOnError(a);
                            ws.OnClose += (e, a) => {
                                InvokeOnClose(a);
                                ClientConnections.Remove(a.Connection);
                            };
                            InvokeOnClientConnected(new OnClientConnectedEventArgs(ws));
                        }
                    }
                }
                ServerStopEvent.Set();
            }

            public override HashSet<WebSocketConnection> GetAllClients()
            {
                return ClientConnections;
            }
        }
    }
}
