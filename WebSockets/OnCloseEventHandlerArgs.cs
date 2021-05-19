using System;

namespace WebSockets {
    public class OnCloseEventHandlerArgs : EventArgs
    {
        public OnCloseEventHandlerArgs(WebSocketConnection connection)
        {
            Connection = connection;
        }

        public WebSocketConnection Connection { get; }
    }
}