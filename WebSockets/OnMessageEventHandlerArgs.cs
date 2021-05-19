using System;

namespace WebSockets {
    public class OnMessageEventHandlerArgs : EventArgs
    {
        public OnMessageEventHandlerArgs(WebSocketConnection connection, string message)
        {
            Connection = connection;
            Message = message;
        }

        public WebSocketConnection Connection { get; }

        public string Message { get; }
    }
}