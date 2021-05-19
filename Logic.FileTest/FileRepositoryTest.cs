using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Data;

using Logic.File;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logic.FileTest
{
    //[TestClass]
    public class FileRepositoryTest
    {
        private const string TEST_FILE = "test.json";
        private const int UPDATE_TIMEOUT = 5000;

        private Client CreateClient()
        {
            return new Client("asdf", "asdf", "asdf", "asdf", 1U, "100 100 100");
        }
        private Product CreateProduct()
        {
            return new Product(0U, "Product", 20.00, ProductType.Toy);
        }

        [TestMethod]
        public void SaveData_ReturnsTrueAndCreatesFile()
        {
            System.IO.File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                Assert.IsTrue(repo.SaveData());
                Assert.IsTrue(System.IO.File.Exists(TEST_FILE));
            }
        }

        [TestMethod]
        public async Task LoadData_ReturnsTrueAndRestoresClient()
        {
            Client testClient = CreateClient();
            System.IO.File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                await repo.CreateClient(
                    testClient.Username,
                    testClient.FirstName,
                    testClient.LastName,
                    testClient.Street,
                    testClient.StreetNumber,
                    testClient.PhoneNumber
                    );
            }
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                Client client = await repo.GetClient(testClient.Username);
                Assert.IsNotNull(client);
                Assert.AreEqual(testClient.FirstName, client.FirstName);
                Assert.AreEqual(testClient.LastName, client.LastName);
                Assert.AreEqual(testClient.Street, client.Street);
                Assert.AreEqual(testClient.StreetNumber, client.StreetNumber);
                Assert.AreEqual(testClient.PhoneNumber, client.PhoneNumber);
            }
        }

        private class TestObserver : IObserver<DataChanged<Client>>, IObserver<DataChanged<Product>>
        {
            public int ClientCompleteCount { get; private set; }
            public Queue<Exception> ClientErrors { get; } = new Queue<Exception>();
            public Queue<DataChanged<Client>> ClientNext { get; } = new Queue<DataChanged<Client>>();
            public int ProductCompleteCount { get; private set; }
            public Queue<Exception> ProductErrors { get; } = new Queue<Exception>();
            public Queue<DataChanged<Product>> ProductNext { get; } = new Queue<DataChanged<Product>>();

            void IObserver<DataChanged<Client>>.OnCompleted()
            {
                ++ClientCompleteCount;
            }

            void IObserver<DataChanged<Client>>.OnError(Exception error)
            {
                ClientErrors.Enqueue(error);
            }

            public void OnNext(DataChanged<Client> value)
            {
                ClientNext.Enqueue(value);
            }

            void IObserver<DataChanged<Product>>.OnCompleted()
            {
                ++ProductCompleteCount;
            }

            void IObserver<DataChanged<Product>>.OnError(Exception error)
            {
                ProductErrors.Enqueue(error);
            }

            public void OnNext(DataChanged<Product> value)
            {
                ProductNext.Enqueue(value);
            }

            public bool ClientsReplaced()
            {
                if (!ClientNext.Any())
                {
                    return false;
                }

                DataChanged<Client> change = ClientNext.Dequeue();
                return change.Action == DataChangedAction.Replace && change.NewItems?.FirstOrDefault() != null;
            }

            public bool ProductsReplaced()
            {
                if (!ProductNext.Any())
                {
                    return false;
                }

                DataChanged<Product> change = ProductNext.Dequeue();
                return change.Action == DataChangedAction.Replace && change.NewItems?.FirstOrDefault() != null;
            }
        }

        [TestMethod]
        public async Task FileRepository_FileChanged_UpdatesData()
        {
            Client testClient = CreateClient();
            Product product = CreateProduct();
            System.IO.File.Delete(TEST_FILE);
            using FileRepository repo = new FileRepository(TEST_FILE);
            await repo.CreateClient(
                testClient.Username,
                testClient.FirstName,
                testClient.LastName,
                testClient.Street,
                testClient.StreetNumber,
                testClient.PhoneNumber
            );
            await repo.CreateProduct(product.Name, product.Price, product.ProductType);
            string fileData = System.IO.File.ReadAllText(TEST_FILE);
            await repo.RemoveClient(testClient);
            await repo.RemoveProduct((await repo.GetAllProducts()).First());
            Assert.AreEqual(0, (await repo.GetAllClients()).Count);
            Assert.AreEqual(0, (await repo.GetAllProducts()).Count);
            bool clientsReplaced = false;
            bool productsReplaced = false;
            TestObserver obs = new TestObserver();
            using IDisposable clientUnsubscriber = repo.Subscribe((IObserver<DataChanged<Client>>)obs);
            using IDisposable productUnsubscriber = repo.Subscribe((IObserver<DataChanged<Product>>)obs);
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
