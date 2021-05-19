using System;

namespace WebSockets {
    public class OnErrorEventHandlerArgs : EventArgs
    {
        public OnErrorEventHandlerArgs(WebSocketConnection connection, Exception exception)
        {
            Connection = connection;
            Exception = exception;
        }

        public WebSocketConnection Connection { get; }

        public Exception Exception { get; }
    }
}