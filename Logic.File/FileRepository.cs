using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Logic
{
    public class FileRepository : IDataRepository, IDisposable
    {

        public FileRepository(string filename = "data.json")
        {
            DataFile = new FileSystemWatcher(filename);
            if (File.Exists(filename))
            {
                LoadData();
            }
            DataFile.Changed += DataFile_Changed;
            DataFile.Deleted += DataFile_Deleted;
        }

        private void DataFile_Deleted(object sender, FileSystemEventArgs e)
        {
            SaveData();
        }

        private void DataFile_Changed(object sender, FileSystemEventArgs e)
        {
            LoadData();
        }

        FileSystemWatcher DataFile
        {
            get;
        }

        public void SaveData()
        {
            RepositoryDTO dto = new RepositoryDTO();
            lock (ClientLock)
            {
                dto.Clients = ClientManager.GetAll().ToList();
            }
            lock (ProductLock)
            {
                dto.Products = ProductManager.GetAll().ToList();
            }
            lock (OrderLock)
            {
                dto.Orders = OrderManager.GetAll().ToList();
            }
            lock (FileLock)
            {
                DataFile.Changed -= DataFile_Changed;
                try
                {
                    File.WriteAllText(DataFile.Path, JsonConvert.SerializeObject(dto, Formatting.Indented));
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
                DataFile.Changed += DataFile_Changed;
            }
        }

        public void LoadData()
        {
            lock (FileLock)
            {
                try
                {
                    RepositoryDTO dto = JsonConvert.DeserializeObject<RepositoryDTO>(File.ReadAllText(DataFile.Path));
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                }
            }
        }

        protected object ClientLock
        {
            get;
        } = new object();

        protected object OrderLock
        {
            get;
        } = new object();

        protected object ProductLock
        {
            get;
        } = new object();

        protected object FileLock
        {
            get;
        } = new object();

        protected ClientManager ClientManager
        {
            get;
        } = new ClientManager();

        protected OrderManager OrderManager
        {
            get;
        } = new OrderManager();

        protected ProductManager ProductManager
        {
            get;
        } = new ProductManager();

        public event NotifyCollectionChangedEventHandler ClientsChanged
        {
            add
            {
                ClientManager.CollectionChanged += value;
            }
            remove
            {
                ClientManager.CollectionChanged -= value;
            }
        }
        public event NotifyCollectionChangedEventHandler OrdersChanged
        {
            add
            {
                OrderManager.CollectionChanged += value;
            }
            remove
            {
                OrderManager.CollectionChanged -= value;
            }
        }
        public event NotifyCollectionChangedEventHandler ProductsChanged
        {
            add
            {
                ProductManager.CollectionChanged += value;
            }
            remove
            {
                ProductManager.CollectionChanged -= value;
            }
        }

        public bool CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            lock (ClientLock)
            {
                return ClientManager.Create(username, firstName, lastName, street, streetNumber, phoneNumber);
            }
        }

        public bool CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(clientUsername) == null)
                {
                    return false;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in productIdQuantityMap)
                    {
                        Product product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return false;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    lock (OrderLock)
                    {
                        return OrderManager.Create(clientUsername, orderDate, productIdQuantityMap, totalPrice, null);
                    }
                }
            }
        }

        public bool CreateProduct(string name, double price, ProductType productType)
        {
            lock (ProductLock)
            {
                return ProductManager.Create(name, price, productType);
            }
        }

        public HashSet<Client> GetAllClients()
        {
            lock (ClientLock)
            {
                return ClientManager.GetAll();
            }
        }

        public HashSet<Order> GetAllOrders()
        {
            lock (OrderLock)
            {
                return OrderManager.GetAll();
            }
        }

        public HashSet<Product> GetAllProducts()
        {
            lock (ProductLock)
            {
                return ProductManager.GetAll();
            }
        }

        public Client GetClient(string username)
        {
            lock (ClientLock)
            {
                return ClientManager.Get(username);
            }
        }

        public Order GetOrder(uint id)
        {
            lock (OrderLock)
            {
                return OrderManager.Get(id);
            }
        }

        public Product GetProduct(uint id)
        {
            lock (ProductLock)
            {
                return ProductManager.Get(id);
            }
        }

        public bool RemoveClient(string username)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    if (ClientManager.Get(username) == null)
                    {
                        return false;
                    }
                    foreach (uint orderId in OrderManager.GetAll().Where(order => order.ClientUsername == username).Select(o => o.Id))
                    {
                        OrderManager.Remove(orderId);
                    }
                    return ClientManager.Remove(username);
                }
            }
        }

        public bool RemoveOrder(uint id)
        {
            lock (OrderLock)
            {
                return OrderManager.Remove(id);
            }
        }

        public bool RemoveProduct(uint id)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    if (ProductManager.Get(id) == null)
                    {
                        return false;
                    }
                    foreach (Order order in OrderManager.GetAll())
                    {
                        if (order.ProductIdQuantityMap.ContainsKey(id))
                        {
                            return false;
                        }
                    }
                    return ProductManager.Remove(id);
                }
            }
        }

        public bool Update(Client client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    return ClientManager.Update(client);
                }
            }
        }

        public bool Update(Order order)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(order.ClientUsername) == null)
                {
                    return false;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in order.ProductIdQuantityMap)
                    {
                        Product product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return false;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    order.Price = totalPrice;
                    lock (OrderLock)
                    {
                        return OrderManager.Update(order);
                    }
                }
            }
        }

        public bool Update(Product product)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    return ProductManager.Update(product);
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DataFile.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
