using System;
using System.Text;
using System.Threading.Tasks;

namespace WebSockets
{
    public abstract class WebSocketConnection : IDisposable
    {
        public event OnMessageEventHandler OnMessage;
        public event OnErrorEventHandler OnError;
        public event OnCloseEventHandler OnClose;

        public abstract Task SendAsync(string message);

        public abstract Task DisconnectAsync();

        protected abstract void Dispose(bool disposing);

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal static ArraySegment<byte> GetArraySegment(string message)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            return new ArraySegment<byte>(buffer);
        }

        protected void InvokeOnMessage(OnMessageEventHandlerArgs args)
        {
            OnMessage?.Invoke(this, args);
        }

        protected void InvokeOnError(OnErrorEventHandlerArgs args)
        {
            OnError?.Invoke(this, args);
        }

        protected void InvokeOnClose(OnCloseEventHandlerArgs args)
        {
            OnClose?.Invoke(this, args);
        }
    }
}
