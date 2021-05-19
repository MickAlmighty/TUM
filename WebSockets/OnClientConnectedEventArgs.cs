using System;

namespace WebSockets {
    public class OnClientConnectedEventArgs : EventArgs
    {
        public OnClientConnectedEventArgs(WebSocketConnection clientConnection)
        {
            ClientConnection = clientConnection;
        }

        public WebSocketConnection ClientConnection { get; }
    }
}