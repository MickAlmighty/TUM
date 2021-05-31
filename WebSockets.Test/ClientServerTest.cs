using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable AccessToDisposedClosure

namespace WebSockets.Test
{
    [TestClass]
    public class ClientServerTest
    {
        private readonly TimeSpan TEST_TIMEOUT = TimeSpan.FromSeconds(5.0);
        private const uint TEST_CONNECTION_PORT = 4444U;
        private readonly Uri TEST_CONNECTION_URI = new Uri($"ws://localhost:{TEST_CONNECTION_PORT}");

        [TestMethod]
        public async Task ConnectDisconnect_ValidStatusAndInvokesConnectionEvents()
        {
            using ServerWebSocketConnection server = WebSocketServer.CreateServer(TEST_CONNECTION_PORT);
            bool connected = false;
            bool serverSideDisconnected = false, clientSideDisconnected = false;
            server.OnClientConnected += (o, a) => connected = true;
            server.OnClose += (o, a) => serverSideDisconnected = true;
            _ = Task.Run(server.RunServerLoop);
            using WebSocketConnection client = await WebSocketClient.ConnectAsync(TEST_CONNECTION_URI);
            client.OnClose += (o, a) => clientSideDisconnected = true;
            Assert.IsTrue(connected);
            Assert.IsFalse(serverSideDisconnected);
            Assert.IsFalse(clientSideDisconnected);
            await client.DisconnectAsync();
            Assert.IsTrue(serverSideDisconnected);
            Assert.IsTrue(clientSideDisconnected);
        }

        [TestMethod]
        public async Task TwoWayCommunication_MessagesReceivedAndValid()
        {
            const string MSG1 = "abcd", MSG2 = "ASDF123", MSG3 = "qwerty", MSG4 = "π≥úÊ ”∆å•£";
            ConcurrentQueue<string> serverMessages = new ConcurrentQueue<string>(), clientMessages = new ConcurrentQueue<string>();
            using AutoResetEvent
                serverMsgEvent = new AutoResetEvent(false),
                clientConnEvent = new AutoResetEvent(false),
                clientMsgEvent = new AutoResetEvent(false);
            using ServerWebSocketConnection server = WebSocketServer.CreateServer(TEST_CONNECTION_PORT);
            server.OnMessage += (o, a) => {
                serverMessages.Enqueue(a.Message);
                serverMsgEvent.Set();
            };
            server.OnClientConnected += (o, a) => clientConnEvent.Set();
            _ = Task.Run(server.RunServerLoop);
            using WebSocketConnection client = await WebSocketClient.ConnectAsync(TEST_CONNECTION_URI);
            client.OnMessage += (o, a) => {
                clientMessages.Enqueue(a.Message);
                clientMsgEvent.Set();
            };
            Assert.IsTrue(clientConnEvent.WaitOne(TEST_TIMEOUT));
            await client.SendAsync(MSG1);
            Assert.IsTrue(serverMsgEvent.WaitOne(TEST_TIMEOUT));
            await client.SendAsync(MSG2);
            Assert.IsTrue(serverMsgEvent.WaitOne(TEST_TIMEOUT));
            await server.SendAsync(MSG3);
            Assert.IsTrue(clientMsgEvent.WaitOne(TEST_TIMEOUT));
            await server.SendAsync(MSG4);
            Assert.IsTrue(clientMsgEvent.WaitOne(TEST_TIMEOUT));
            Assert.AreEqual(2, serverMessages.Count);
            Assert.AreEqual(2, clientMessages.Count);
            Assert.IsTrue(serverMessages.TryDequeue(out string msg));
            Assert.AreEqual(MSG1, msg);
            Assert.IsTrue(serverMessages.TryDequeue(out msg));
            Assert.AreEqual(MSG2, msg);
            Assert.IsTrue(clientMessages.TryDequeue(out msg));
            Assert.AreEqual(MSG3, msg);
            Assert.IsTrue(clientMessages.TryDequeue(out msg));
            Assert.AreEqual(MSG4, msg);
        }

        [TestMethod]
        public async Task MultipleClients_MessagesDistributedProperly()
        {
            const string MSG = "123";
            List<WebSocketConnection> serverSideClients = new List<WebSocketConnection>();
            uint serverMessages = 0U, c1Messages = 0U, c2Messages = 0U, c3Messages = 0U;
            using AutoResetEvent
                serverMsgEvent = new AutoResetEvent(false),
                clientConnEvent = new AutoResetEvent(false),
                client1MsgEvent = new AutoResetEvent(false),
                client2MsgEvent = new AutoResetEvent(false),
                client3MsgEvent = new AutoResetEvent(false);
            using (ServerWebSocketConnection server = WebSocketServer.CreateServer(TEST_CONNECTION_PORT))
            {
                _ = Task.Run(server.RunServerLoop);
                server.OnMessage += (o, a) => {
                    ++serverMessages;
                    serverMsgEvent.Set();
                };
                server.OnClientConnected += (o, a) => {
                    serverSideClients.Add(a.ClientConnection);
                    clientConnEvent.Set();
                };
                using WebSocketConnection client1 = await WebSocketClient.ConnectAsync(TEST_CONNECTION_URI);
                client1.OnMessage += (o, a) => {
                    ++c1Messages;
                    client1MsgEvent.Set();
                };
                Assert.IsTrue(clientConnEvent.WaitOne(TEST_TIMEOUT));
                using WebSocketConnection client2 = await WebSocketClient.ConnectAsync(TEST_CONNECTION_URI);
                client2.OnMessage += (o, a) => {
                    ++c2Messages;
                    client2MsgEvent.Set();
                };
                Assert.IsTrue(clientConnEvent.WaitOne(TEST_TIMEOUT));
                using WebSocketConnection client3 = await WebSocketClient.ConnectAsync(TEST_CONNECTION_URI);
                client3.OnMessage += (o, a) => {
                    ++c3Messages;
                    client3MsgEvent.Set();
                };
                Assert.IsTrue(clientConnEvent.WaitOne(TEST_TIMEOUT));
                await serverSideClients[0].SendAsync(MSG);
                Assert.IsTrue(client1MsgEvent.WaitOne(TEST_TIMEOUT));
                await serverSideClients[1].SendAsync(MSG);
                Assert.IsTrue(client2MsgEvent.WaitOne(TEST_TIMEOUT));
                await serverSideClients[2].SendAsync(MSG);
                Assert.IsTrue(client3MsgEvent.WaitOne(TEST_TIMEOUT));
                await server.SendAsync(MSG);
                Assert.IsTrue(client1MsgEvent.WaitOne(TEST_TIMEOUT));
                Assert.IsTrue(client2MsgEvent.WaitOne(TEST_TIMEOUT));
                Assert.IsTrue(client3MsgEvent.WaitOne(TEST_TIMEOUT));
                await client1.SendAsync(MSG);
                Assert.IsTrue(serverMsgEvent.WaitOne(TEST_TIMEOUT));
                await client2.SendAsync(MSG);
                Assert.IsTrue(serverMsgEvent.WaitOne(TEST_TIMEOUT));
                await client3.SendAsync(MSG);
                Assert.IsTrue(serverMsgEvent.WaitOne(TEST_TIMEOUT));
            }
            Assert.AreEqual(3U, serverMessages);
            Assert.AreEqual(2U, c1Messages);
            Assert.AreEqual(2U, c2Messages);
            Assert.AreEqual(2U, c3Messages);
        }
    }
}
