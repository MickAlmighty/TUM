using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Server
{
    // Reference: https://github.com/mpostol/NBlockchain/blob/master/P2PPrototocol/NodeJSAPI/WebSocketServer.cs
    internal static class WebSocketServer
    {
        internal static ArraySegment<byte> GetArraySegment(this string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            return new ArraySegment<byte>(buffer);
        }

        internal static async Task Server(WebReceiver webReceiver, int p2p_port, Action<WebSocketConnection> onConnection)
        {
            Uri _uri = new Uri($@"http://localhost:{p2p_port}/");
            await ServerLoop(webReceiver, _uri, onConnection);
        }

        private static async Task ServerLoop(WebReceiver webReceiver, Uri _uri, Action<WebSocketConnection> onConnection)
        {
            HttpListener _server = new HttpListener();
            _server.Prefixes.Add(_uri.ToString());
            _server.Start();
            while (true)
            {
                HttpListenerContext _hc = await _server.GetContextAsync();
                if (!_hc.Request.IsWebSocketRequest)
                {
                    _hc.Response.StatusCode = 400;
                    _hc.Response.Close();
                }
                HttpListenerWebSocketContext _context = await _hc.AcceptWebSocketAsync(null);
                WebSocketConnection ws = new ServerWebSocketConnection(webReceiver, _context.WebSocket, _hc.Request.RemoteEndPoint);
                onConnection?.Invoke(ws);
            }
        }
        private sealed class ServerWebSocketConnection : WebSocketConnection
        {

            public ServerWebSocketConnection(WebReceiver webReceiver, WebSocket webSocket, IPEndPoint remoteEndPoint)
            {
                if (webReceiver != null)
                {
                    OnMessage = msg => webReceiver.OnMessage(this, msg);
                    OnError = () => webReceiver.OnError(this);
                    OnClose = () => webReceiver.OnClose(this);
                }
                m_WebSocket = webSocket;
                m_remoteEndPoint = remoteEndPoint;
                Task.Factory.StartNew(() => ServerMessageLoop(webSocket));
            }

            #region WebSocketConnection
            protected override Task SendTask(string message)
            {
                return m_WebSocket.SendAsync(message.GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            internal override Task DisconnectAsync()
            {
                return m_WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
            }
            #endregion

            #region Object
            public override string ToString()
            {
                return m_remoteEndPoint.ToString();
            }
            #endregion

            private WebSocket m_WebSocket = null;
            private IPEndPoint m_remoteEndPoint;
            private void ServerMessageLoop(WebSocket ws)
            {
                byte[] buffer = new byte[1024 * 32];
                while (true)
                {
                    ArraySegment<byte> _segments = new ArraySegment<byte>(buffer);
                    WebSocketReceiveResult _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
                    if (_receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        OnClose?.Invoke();
                        ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
                        return;
                    }
                    int count = _receiveResult.Count;
                    while (!_receiveResult.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            OnClose?.Invoke();
                            ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                            return;
                        }
                        _segments = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        _receiveResult = ws.ReceiveAsync(_segments, CancellationToken.None).Result;
                        count += _receiveResult.Count;
                    }
                    string _message = Encoding.UTF8.GetString(buffer, 0, count);
                    OnMessage?.Invoke(_message);
                }
            }

        }
    }
}
