using Data;
using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;

using Logic.File;

namespace LogicTest
{
    [TestClass]
    public class FileRepositoryTest
    {
        private const string TEST_FILE = "test.json";
        private const int UPDATE_TIMEOUT = 500;

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
            File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                Assert.IsTrue(repo.SaveData());
                Assert.IsTrue(File.Exists(TEST_FILE));
            }
        }

        [TestMethod]
        public void LoadData_ReturnsTrueAndRestoresClient()
        {
            Client testClient = CreateClient();
            File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                repo.CreateClient(
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
                Client client = repo.GetClient(testClient.Username);
                Assert.IsNotNull(client);
                Assert.AreEqual(testClient.FirstName, client.FirstName);
                Assert.AreEqual(testClient.LastName, client.LastName);
                Assert.AreEqual(testClient.Street, client.Street);
                Assert.AreEqual(testClient.StreetNumber, client.StreetNumber);
                Assert.AreEqual(testClient.PhoneNumber, client.PhoneNumber);
            }
        }

        [TestMethod]
        public void FileRepository_FileChanged_UpdatesData()
        {
            Client testClient = CreateClient();
            Product product = CreateProduct();
            File.Delete(TEST_FILE);
            using (FileRepository repo = new FileRepository(TEST_FILE))
            {
                repo.CreateClient(
                    testClient.Username,
                    testClient.FirstName,
                    testClient.LastName,
                    testClient.Street,
                    testClient.StreetNumber,
                    testClient.PhoneNumber
                    );
                repo.CreateProduct(product.Name, product.Price, product.ProductType);
                string fileData = File.ReadAllText(TEST_FILE);
                repo.RemoveClient(testClient.Username);
                repo.RemoveProduct(repo.GetAllProducts().First().Id);
                Assert.AreEqual(0, repo.GetAllClients().Count);
                Assert.AreEqual(0, repo.GetAllProducts().Count);
                bool clientsChanged = false;
                bool productsChanged = false;
                repo.ClientsChanged += (o, e) =>
                {
                    if (e.Action == NotifyDataChangedAction.Replace && e.NewItems?.Cast<Client>().FirstOrDefault() != null)
                    {
                        clientsChanged = true;
                    }
                };
                repo.ProductsChanged += (o, e) =>
                {
                    if (e.Action == NotifyDataChangedAction.Replace && e.NewItems?.Cast<Product>().FirstOrDefault() != null)
                    {
                        productsChanged = true;
                    }
                };
                File.WriteAllText(TEST_FILE, fileData);
                SpinWait.SpinUntil(() => clientsChanged && productsChanged, UPDATE_TIMEOUT);
                Assert.IsTrue(clientsChanged);
                Assert.IsTrue(productsChanged);
            }
        }
    }
}
