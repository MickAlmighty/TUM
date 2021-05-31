using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Data;

using Logic;
using Logic.Client;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Presentation.Server;
// ReSharper disable AccessToDisposedClosure

namespace IntegrationTest
{
    [TestClass]
    public class IntegrationTest
    {
        private readonly TimeSpan TEST_TIMEOUT = TimeSpan.FromSeconds(5.0);
        private const uint TEST_PORT = 4444U;

        private class TestObserver : IObserver<DataChanged<IClient>>, IObserver<DataChanged<IProduct>>
        {
            public int ClientCompleteCount { get; set; }
            public Queue<Exception> ClientErrors { get; } = new Queue<Exception>();
            public Queue<DataChanged<IClient>> ClientNext { get; } = new Queue<DataChanged<IClient>>();
            public int ProductCompleteCount { get; private set; }
            public Queue<Exception> ProductErrors { get; } = new Queue<Exception>();
            public Queue<DataChanged<IProduct>> ProductNext { get; } = new Queue<DataChanged<IProduct>>();

            void IObserver<DataChanged<IClient>>.OnCompleted()
            {
                ++ClientCompleteCount;
            }

            void IObserver<DataChanged<IClient>>.OnError(Exception error)
            {
                ClientErrors.Enqueue(error);
            }

            public void OnNext(DataChanged<IClient> value)
            {
                ClientNext.Enqueue(value);
            }

            void IObserver<DataChanged<IProduct>>.OnCompleted()
            {
                ++ProductCompleteCount;
            }

            void IObserver<DataChanged<IProduct>>.OnError(Exception error)
            {
                ProductErrors.Enqueue(error);
            }

            public void OnNext(DataChanged<IProduct> value)
            {
                ProductNext.Enqueue(value);
            }
        }

        [TestMethod]
        public async Task Server_ClientWebRepository_CommunicationValid()
        {
            using Server server = new Server(TEST_PORT, new DataRepository(), null);
            using AutoResetEvent
                clientConnEvent = new AutoResetEvent(false),
                clientDisconnEvent = new AutoResetEvent(false);
            server.ServerWebSocketConnection.OnError += (o, a) => Assert.Fail($"Server error! {a.Exception}");
            server.ServerWebSocketConnection.OnClientConnected += (o, a) => clientConnEvent.Set();
            server.ServerWebSocketConnection.OnClose += (o, a) => clientDisconnEvent.Set();
            Task serverTask = Task.Run(server.RunServer);
            using (WebRepository client = new WebRepository())
            {
                Assert.IsTrue(await client.OpenRepository(ClientUtil.CreateLocalConnectionString(TEST_PORT)));
                Assert.IsTrue(clientConnEvent.WaitOne(TEST_TIMEOUT));
                Assert.AreEqual((await server.DataRepository.GetAllClients()).Count, (await client.GetAllClients()).Count);
                Assert.AreEqual((await server.DataRepository.GetAllProducts()).Count, (await client.GetAllProducts()).Count);
                Assert.AreEqual((await server.DataRepository.GetAllOrders()).Count, (await client.GetAllOrders()).Count);
                TestObserver testObserver = new TestObserver();
                using (IDisposable _ = server.DataRepository.Subscribe((IObserver<DataChanged<IClient>>)testObserver),
                    __ = server.DataRepository.Subscribe((IObserver<DataChanged<IProduct>>)testObserver))
                {
                    await client.RemoveClient((await client.GetAllClients()).First());
                    await client.RemoveProduct((await client.GetAllProducts()).First());
                }
                Assert.AreEqual(0, testObserver.ClientErrors.Count);
                Assert.AreEqual(0, testObserver.ProductErrors.Count);
                Assert.AreEqual(1, testObserver.ClientNext.Count);
                Assert.AreEqual(1, testObserver.ProductNext.Count);
                Assert.AreEqual(DataChangedAction.Remove, testObserver.ClientNext.Dequeue().Action);
                Assert.AreEqual(DataChangedAction.Remove, testObserver.ProductNext.Dequeue().Action);
            }
            Assert.IsTrue(clientDisconnEvent.WaitOne(TEST_TIMEOUT));
        }
    }
}
