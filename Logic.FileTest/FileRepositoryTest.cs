using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Data;

using DataModel;

using Logic.File;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic.FileTest
{
    [TestClass]
    public class FileRepositoryTest
    {
        private const string TEST_FILE = "test.json";
        private const int UPDATE_TIMEOUT = 5000;

        private IClient CreateClient()
        {
            return new Client("asdf", "asdf", "asdf", "asdf", 1U, "100 100 100");
        }
        private IProduct CreateProduct()
        {
            return new Product(0U, "Product", 20.00, ProductType.Toy);
        }

        [TestMethod]
        public void SaveData_ReturnsTrueAndCreatesFile()
        {
            System.IO.File.Delete(TEST_FILE);
            using FileRepository repo = new FileRepository();
            Assert.IsTrue(repo.OpenRepository(TEST_FILE).GetAwaiter().GetResult());
            Assert.IsTrue(repo.SaveData());
            Assert.IsTrue(System.IO.File.Exists(TEST_FILE));
        }

        [TestMethod]
        public async Task LoadData_ReturnsTrueAndRestoresClient()
        {
            IClient testClient = CreateClient();
            System.IO.File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository())
            {
                Assert.IsTrue(repo.OpenRepository(TEST_FILE).GetAwaiter().GetResult());
                await repo.CreateClient(
                    testClient.Username,
                    testClient.FirstName,
                    testClient.LastName,
                    testClient.Street,
                    testClient.StreetNumber,
                    testClient.PhoneNumber);
            }
            using (FileRepository repo = new FileRepository())
            {
                Assert.IsTrue(repo.OpenRepository(TEST_FILE).GetAwaiter().GetResult());
                IClient client = await repo.GetClient(testClient.Username);
                Assert.IsNotNull(client);
                Assert.AreEqual(testClient.FirstName, client.FirstName);
                Assert.AreEqual(testClient.LastName, client.LastName);
                Assert.AreEqual(testClient.Street, client.Street);
                Assert.AreEqual(testClient.StreetNumber, client.StreetNumber);
                Assert.AreEqual(testClient.PhoneNumber, client.PhoneNumber);
            }
        }

        private class TestObserver : IObserver<DataChanged<IClient>>, IObserver<DataChanged<IProduct>>
        {
            private int ClientCompleteCount { get; set; }
            private Queue<Exception> ClientErrors { get; } = new Queue<Exception>();
            private Queue<DataChanged<IClient>> ClientNext { get; } = new Queue<DataChanged<IClient>>();
            private int ProductCompleteCount { get; set; }
            private Queue<Exception> ProductErrors { get; } = new Queue<Exception>();
            private Queue<DataChanged<IProduct>> ProductNext { get; } = new Queue<DataChanged<IProduct>>();

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

            public bool ClientsReplaced()
            {
                if (!ClientNext.Any())
                {
                    return false;
                }

                DataChanged<IClient> change = ClientNext.Dequeue();
                return change.Action == DataChangedAction.Replace && change.NewItems?.FirstOrDefault() != null;
            }

            public bool ProductsReplaced()
            {
                if (!ProductNext.Any())
                {
                    return false;
                }

                DataChanged<IProduct> change = ProductNext.Dequeue();
                return change.Action == DataChangedAction.Replace && change.NewItems?.FirstOrDefault() != null;
            }
        }

        [TestMethod]
        public async Task FileRepository_FileChanged_UpdatesData()
        {
            IClient testClient = CreateClient();
            IProduct product = CreateProduct();
            System.IO.File.Delete(TEST_FILE);
            using FileRepository repo = new FileRepository();
            Assert.IsTrue(repo.OpenRepository(TEST_FILE).GetAwaiter().GetResult());
            await repo.CreateClient(
                testClient.Username,
                testClient.FirstName,
                testClient.LastName,
                testClient.Street,
                testClient.StreetNumber,
                testClient.PhoneNumber);
            await repo.CreateProduct(product.Name, product.Price, product.ProductType);
            string fileData = System.IO.File.ReadAllText(TEST_FILE);
            await repo.RemoveClient(testClient);
            await repo.RemoveProduct((await repo.GetAllProducts()).First());
            Assert.AreEqual(0, (await repo.GetAllClients()).Count);
            Assert.AreEqual(0, (await repo.GetAllProducts()).Count);
            bool clientsReplaced = false;
            bool productsReplaced = false;
            TestObserver obs = new TestObserver();
            using IDisposable clientUnsubscriber = repo.Subscribe((IObserver<DataChanged<IClient>>)obs);
            using IDisposable productUnsubscriber = repo.Subscribe((IObserver<DataChanged<IProduct>>)obs);
            System.IO.File.WriteAllText(TEST_FILE, fileData);
            SpinWait.SpinUntil(() => {
                clientsReplaced |= obs.ClientsReplaced();
                productsReplaced |= obs.ProductsReplaced();
                return clientsReplaced && productsReplaced;
            }, UPDATE_TIMEOUT);
            Assert.IsTrue(clientsReplaced);
            Assert.IsTrue(productsReplaced);
        }
    }
}
