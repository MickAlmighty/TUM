using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebSockets
{
    public abstract class ServerWebSocketConnection : WebSocketConnection
    {
        public abstract Task RunServerLoop();

        public abstract HashSet<WebSocketConnection> GetAllClients();

        public event OnClientConnectedEventHandler OnClientConnected;

        protected void InvokeOnClientConnected(OnClientConnectedEventArgs args)
        {
            OnClientConnected?.Invoke(this, args);
        }
    }
}
