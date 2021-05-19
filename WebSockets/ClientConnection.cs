using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets
{
    internal sealed class ClientConnection : WebSocketConnection
    {
        private CancellationTokenSource TokenSource { get; } = new CancellationTokenSource();
        private Task ClientLoopTask { get; }
        private WebSocket WebSocket { get; }
        private string ClientDescription { get; }

        public ClientConnection(WebSocket webSocket, string clientDescription)
        {
            WebSocket = webSocket;
            ClientDescription = clientDescription;
            ClientLoopTask = Task.Run(RunClientLoop);
        }

        public override async Task SendAsync(string message)
        {
            Console.WriteLine($"{ClientDescription}< {message}");
            await WebSocket.SendAsync(GetArraySegment(message), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public override async Task DisconnectAsync()
        {
            TokenSource.Cancel();
            await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Shutdown procedure started", CancellationToken.None);
            ClientLoopTask.Wait();
        }

        protected override void Dispose(bool disposing)
        {
            if (!TokenSource.IsCancellationRequested)
            {
                Task.Run(DisconnectAsync).Wait();
            }
        }

        public override string ToString()
        {
            return ClientDescription;
        }

        private async Task RunClientLoop()
        {
            Task cancelTask = Task.Run(() => WaitHandle.WaitAny(new[] { TokenSource.Token.WaitHandle }));
            try
            {
                byte[] buffer = new byte[1024 * 32];
                while (!TokenSource.IsCancellationRequested)
                {
                    ArraySegment<byte> segment = new ArraySegment<byte>(buffer);
                    Task<WebSocketReceiveResult> receiveResultTask = WebSocket.ReceiveAsync(segment, CancellationToken.None);
                    int index = Task.WaitAny(cancelTask, receiveResultTask);
                    if (index == 0)
                    {
                        break;
                    }

                    WebSocketReceiveResult result = receiveResultTask.Result;
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        InvokeOnClose(new OnCloseEventHandlerArgs(this));
                        await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "I am closing", CancellationToken.None);
                        return;
                    }
                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            InvokeOnClose(new OnCloseEventHandlerArgs(this));
                            await WebSocket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData, "That's too long", CancellationToken.None);
                            return;
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await WebSocket.ReceiveAsync(segment, CancellationToken.None);
                        count += result.Count;
                    }
                    string message = Encoding.UTF8.GetString(buffer, 0, count);
                    InvokeOnMessage(new OnMessageEventHandlerArgs(this, message));
                }
            }
            catch (Exception e)
            {
                InvokeOnError(new OnErrorEventHandlerArgs(this, e));
                await WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, "Connection has been broken because of an exception", CancellationToken.None);
            }

            TokenSource.Cancel();
            cancelTask.Wait();
        }
    }
}