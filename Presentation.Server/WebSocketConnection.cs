using System;
using System.Threading.Tasks;

namespace Presentation.Server
{
    // Reference: https://github.com/mpostol/NBlockchain/blob/master/P2PPrototocol/NodeJSAPI/WebSocketConnection.cs
    internal abstract class WebSocketConnection
    {
        public virtual Action<string> OnMessage { set; protected get; } = x => { };
        public virtual Action OnClose { set; protected get; } = () => { };
        public virtual Action OnError { set; protected get; } = () => { };
        internal async Task SendAsync(string message)
        {
            await SendTask(message);
        }
        protected abstract Task SendTask(string message);
        internal abstract Task DisconnectAsync();
    }
}