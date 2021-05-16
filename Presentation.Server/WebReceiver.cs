namespace Presentation.Server
{
    interface WebReceiver
    {
        void OnMessage(WebSocketConnection connection, string message);
        void OnClose(WebSocketConnection connection);
        void OnError(WebSocketConnection connection);
    }
}
