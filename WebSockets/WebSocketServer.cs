using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets
{
    // Reference: https://github.com/mpostol/NBlockchain/blob/master/P2PPrototocol/NodeJSAPI/WebSocketServer.cs
    public static class WebSocketServer
    {
        public static ServerWebSocketConnection CreateServer(uint port)
        {
            Uri uri = new Uri($@"http://localhost:{port}/");
            return new ServerConnection(uri);
        }

        private sealed class ServerConnection : ServerWebSocketConnection
        {
            private HashSet<WebSocketConnection> ClientConnections { get; } = new HashSet<WebSocketConnection>();

            private CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();

            private Uri Uri { get; }
            private bool ServerStarted { get; set; }

            public override bool IsRunning
            {
                get
                {
                    return ServerStarted && !ServerStopEvent.WaitOne(0);
                }
            }

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

            public override Task DisconnectAsync()
            {
                if (!ServerStarted || ServerStopEvent.WaitOne(0))
                {
                    return Task.CompletedTask;
                }
                TokenSource.Cancel();
                ServerStopEvent.WaitOne();
                foreach (WebSocketConnection conn in ClientConnections)
                {
                    Task.Run(conn.DisconnectAsync);
                }
                ClientConnections.Clear();
                return Task.CompletedTask;
            }

            protected override void Dispose(bool disposing)
            {
                Task.Run(DisconnectAsync).Wait();
            }

            public override async Task RunServerLoop()
            {
                if (ServerStarted)
                {
                    return;
                }
                ServerStarted = true;
                Task cancelTask = Task.Run(() => WaitHandle.WaitAny(new[] { TokenSource.Token.WaitHandle }));
                using (HttpListener listener = new HttpListener())
                {
                    listener.Prefixes.Add(Uri.ToString());
                    listener.Start();
                    while (!TokenSource.IsCancellationRequested)
                    {
                        Task<HttpListenerContext> listenerContextTask = listener.GetContextAsync();
                        int index = Task.WaitAny(cancelTask, listenerContextTask);
                        if (index == 0)
                        {
                            break;
                        }

                        HttpListenerContext listenerContext = listenerContextTask.Result;
                        if (!listenerContext.Request.IsWebSocketRequest)
                        {
                            listenerContext.Response.StatusCode = 400;
                            listenerContext.Response.Close();
                        }
                        else
                        {
                            HttpListenerWebSocketContext webSocketContext = await listenerContext.AcceptWebSocketAsync(null);
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

                cancelTask.Wait();
                ServerStopEvent.Set();
            }

            public override HashSet<WebSocketConnection> GetAllClients()
            {
                return ClientConnections;
            }
        }
    }
}
