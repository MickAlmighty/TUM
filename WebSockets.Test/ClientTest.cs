using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebSockets.Test
{
    [TestClass]
    public class ClientTest
    {
        private const uint TEST_CONNECTION_PORT = 4444U;
        private readonly Uri TEST_CONNECTION_URI = new Uri($"ws://localhost:{TEST_CONNECTION_PORT}");

        [TestMethod]
        public async Task ConnectAsync_InvalidUri_Throws()
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => WebSocketClient.ConnectAsync(new Uri("http://localhost:1234/")));
        }

        [TestMethod]
        public async Task ConnectAsync_NoServer_Throws()
        {
            await Assert.ThrowsExceptionAsync<WebSocketException>(() => WebSocketClient.ConnectAsync(TEST_CONNECTION_URI));
        }
    }
}
