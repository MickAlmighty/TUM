using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logic.Client
{
    // Reference: https://github.com/mpostol/NBlockchain/blob/master/P2PPrototocol/NodeJSAPI/WebSocketClient.cs
    internal static class WebSocketClient
    {
        internal static ArraySegment<byte> GetArraySegment(this string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            return new ArraySegment<byte>(buffer);
        }

        internal static async Task Connect(WebReceiver webReceiver, Uri peer, Action<WebSocketConnection> onOpen, Action<string> log)
        {
            ClientWebSocket m_ClientWebSocket = new ClientWebSocket();
            await m_ClientWebSocket.ConnectAsync(peer, CancellationToken.None);
            switch (m_ClientWebSocket.State)
            {
                case WebSocketState.Open:
                    log($"Opening WebSocket connection to remote server {peer}");
                    WebSocketConnection _socket = new ClientWebSocketConnection(webReceiver, m_ClientWebSocket, peer, log);
                    onOpen?.Invoke(_socket);
                    break;
                default:
                    log?.Invoke($"Cannot connect to remote node status {m_ClientWebSocket.State}");
                    break;
            }
        }

        #region private
        private sealed class ClientWebSocketConnection : WebSocketConnection
        {
            public ClientWebSocketConnection(WebReceiver webReceiver, ClientWebSocket clientWebSocket, Uri peer, Action<string> log)
            {
                if (webReceiver != null)
                {
                    OnMessage = webReceiver.OnMessage;
                    OnError = webReceiver.OnError;
                    OnClose = webReceiver.OnClose;
                }
                this.m_ClientWebSocket = clientWebSocket;
                this.m_Peer = peer;
                this.m_Log = log;
                Task.Factory.StartNew(ClientMessageLoop);
            }

            #region WebSocketConnection
            protected override Task SendTask(string message)
            {
                return m_ClientWebSocket.SendAsync(message.GetArraySegment(), WebSocketMessageType.Text, true, CancellationToken.None); ;
            }
            internal override Task DisconnectAsync()
            {
                return m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
            }
            #endregion

            #region Object
            public override string ToString()
            {
                return m_Peer.ToString();
            }
            #endregion

            #region private
            private ClientWebSocket m_ClientWebSocket = null;
            private Uri m_Peer = null;
            private Action<string> m_Log;
            private void ClientMessageLoop()
            {
                try
                {
                    byte[] buffer = new byte[1024 * 32];
                    while (true)
                    {
                        ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                        WebSocketReceiveResult result = m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            OnClose?.Invoke();
                            m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None).Wait();
                            return;
                        }
                        int count = result.Count;
                        while (!result.EndOfMessage)
                        {
                            if (count >= buffer.Length)
                            {
                                OnClose?.Invoke();
                                m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None).Wait();
                                return;
                            }
                            segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                            result = m_ClientWebSocket.ReceiveAsync(segment, CancellationToken.None).Result;
                            count += result.Count;
                        }
                        string _message = Encoding.UTF8.GetString(buffer, 0, count);
                        OnMessage?.Invoke(_message);
                    }
                }
                catch (Exception _ex)
                {
                    m_Log($"Connection has been broken because of an exception {_ex}");
                    m_ClientWebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None).Wait();
                }
            }
            #endregion
        }
        #endregion
    }
}
