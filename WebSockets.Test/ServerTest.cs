using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebSockets.Test
{
    [TestClass]
    public class ServerTest
    {
        private readonly TimeSpan TEST_TIMEOUT = TimeSpan.FromSeconds(5.0);
        private const uint TEST_CONNECTION_PORT = 4444U;

        [TestMethod]
        public async Task IsRunning_SingleCycle_ReturnsValidStatus()
        {
            using ServerWebSocketConnection server = WebSocketServer.CreateServer(TEST_CONNECTION_PORT);
            Assert.IsFalse(server.IsRunning);
            Task serverTask = Task.Run(server.RunServerLoop);
            // ReSharper disable once AccessToDisposedClosure
            Assert.IsTrue(SpinWait.SpinUntil(() => server.IsRunning, TEST_TIMEOUT));
            await server.DisconnectAsync();
            Assert.IsTrue(serverTask.Wait(TEST_TIMEOUT));
            Assert.IsFalse(server.IsRunning);
        }
    }
}
