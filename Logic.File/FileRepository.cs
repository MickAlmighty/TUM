using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Data;

using Newtonsoft.Json;

namespace Logic.File
{
    public class FileRepository : IDataRepository, IDisposable, IObserver<DataChanged<Order>>
    {
        public FileRepository(string filePath = "data.json")
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            if (filePath.Length == 0)
            {
                throw new ArgumentException(nameof(filePath));
            }

            DataPathWatcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(filePath)) ?? throw new InvalidOperationException());
            FileName = Path.GetFileName(filePath);
            FullFilePath = Path.Combine(DataPathWatcher.Path, FileName);
            if (System.IO.File.Exists(filePath))
            {
                LoadData();
            }
            DataPathWatcher.Changed += DataFile_Changed;
            DataPathWatcher.Deleted += DataFile_Deleted;
            DataPathWatcher.EnableRaisingEvents = true;
            OrderUnsubscriber = OrderManager.Subscribe(this);
        }

        public void OnCompleted() { }
        public void OnError(Exception error)
        {
            Console.WriteLine($"Order subscription error: {error}.");
        }

        public void OnNext(DataChanged<Order> value)
        {
            if (value.Action == DataChangedAction.Add)
            {
                List<uint> newOrders = value.NewItems.Select(o => o.Id).ToList();
                Task.Run(async delegate
                {
                    await Task.Delay(5000);
                    lock (OrderLock)
                    {
                        foreach (uint id in newOrders)
                        {
                            Order order = OrderManager.Get(id);
                            if (order != null && !order.DeliveryDate.HasValue)
                            {
                                order.DeliveryDate = DateTime.Now;
                                foreach (IObserver<OrderSent> observer in OrderSentObservers.ToList())
                                {
                                    observer.OnNext(new OrderSent(order));
                                }
                                Update(order);
                            }
                        }
                    }
                });
            }
        }

        private void DataFile_Deleted(object sender, FileSystemEventArgs e)
        {
            if (e.Name == FileName)
            {
                SaveData();
            }
        }

        private void DataFile_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.Name == FileName)
            {
                LoadData();
            }
        }

        FileSystemWatcher DataPathWatcher
        {
            get;
        }

        string FileName
        {
            get;
        }

        string FullFilePath
        {
            get;
        }

        public bool SaveData()
        {
            RepositoryDTO dto = new RepositoryDTO();
            lock (ClientLock)
            {
                dto.Clients = ClientManager.GetAll();
            }
            lock (ProductLock)
            {
                dto.Products = ProductManager.GetAll();
            }
            lock (OrderLock)
            {
                dto.Orders = OrderManager.GetAll();
            }
            lock (FileLock)
            {
                DataPathWatcher.EnableRaisingEvents = false;
                bool success = true;
                try
                {
                    // TODO: provide a serialization interface and use it
                    System.IO.File.WriteAllText(FullFilePath, JsonConvert.SerializeObject(dto, Formatting.Indented));
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                    success = false;
                }
                DataPathWatcher.EnableRaisingEvents = true;
                return success;
            }
        }

        public bool LoadData()
        {
            lock (FileLock)
            {
                try
                {
                    // TODO: provide a serialization interface and use it
                    RepositoryDTO dto = JsonConvert.DeserializeObject<RepositoryDTO>(System.IO.File.ReadAllText(FullFilePath));
                    lock (ClientLock)
                    {
                        lock (ProductLock)
                        {
                            lock (OrderLock)
                            {
                                ClientManager.ReplaceData(dto.Clients);
                                OrderManager.ReplaceData(dto.Orders);
                                ProductManager.ReplaceData(dto.Products);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
                    return false;
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

        private IDisposable OrderUnsubscriber { get; }

        public bool CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            lock (ClientLock)
            {
                if (ClientManager.Create(username, firstName, lastName, street, streetNumber, phoneNumber))
                {
                    SaveData();
                    return true;
                }

                return false;
            }
        }

        public bool CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate)
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
                        if (OrderManager.Create(clientUsername, orderDate, productIdQuantityMap, totalPrice,
                            deliveryDate))
                        {
                            SaveData();
                            return true;
                        }

                        return false;
                    }
                }
            }
        }

        public bool CreateProduct(string name, double price, ProductType productType)
        {
            lock (ProductLock)
            {
                if (ProductManager.Create(name, price, productType))
                {
                    SaveData();
                    return true;
                }

                return false;
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

                    bool changed = false;
                    foreach (uint orderId in OrderManager.GetAll().Where(order => order.ClientUsername == username).Select(o => o.Id))
                    {
                        changed |= OrderManager.Remove(orderId);
                    }

                    if (ClientManager.Remove(username))
                    {
                        SaveData();
                        return true;
                    }

                    if (changed)
                    {
                        SaveData();
                    }
                    return false;
                }
            }
        }

        public bool RemoveOrder(uint id)
        {
            lock (OrderLock)
            {
                if (OrderManager.Remove(id))
                {
                    SaveData();
                    return true;
                }

                return false;
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

                    if (ProductManager.Remove(id))
                    {
                        SaveData();
                        return true;
                    }

                    return false;
                }
            }
        }

        public bool Update(Client client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    if (ClientManager.Update(client))
                    {
                        SaveData();
                        return true;
                    }

                    return false;
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
                        if (OrderManager.Update(order))
                        {
                            SaveData();
                            return true;
                        }

                        return false;
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
                    if (ProductManager.Update(product))
                    {
                        SaveData();
                        return true;
                    }

                    return false;
                }
            }
        }

        #region IObservable implementation

        private HashSet<IObserver<OrderSent>> OrderSentObservers { get; } = new HashSet<IObserver<OrderSent>>();

        public IDisposable Subscribe(IObserver<OrderSent> observer)
        {
            OrderSentObservers.Add(observer);
            return new Unsubscriber<OrderSent>(OrderSentObservers, observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Client>> observer)
        {
            return ClientManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Product>> observer)
        {
            return ProductManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<Order>> observer)
        {
            return OrderManager.Subscribe(observer);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OrderUnsubscriber.Dispose();
                    DataPathWatcher.Dispose();
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
