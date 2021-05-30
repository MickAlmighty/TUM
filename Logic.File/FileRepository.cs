using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Data;

using DataModel;
using DataModel.Transfer;

using Newtonsoft.Json;

namespace Logic.File
{
    public class FileRepository : IDataRepository, IDisposable, IObserver<DataChanged<IOrder>>
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
            OrderUnsubscriber = OrderManager.Subscribe(this);
        }

        public void OnCompleted() { }
        public void OnError(Exception error)
        {
            Console.WriteLine($"Order subscription error: {error}.");
        }

        public void OnNext(DataChanged<IOrder> value)
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
                            IOrder order = OrderManager.Get(id);
                            if (order != null && !order.DeliveryDate.HasValue)
                            {
                                order.DeliveryDate = DateTime.Now;
                                foreach (IObserver<OrderSent> observer in OrderSentObservers.ToList())
                                {
                                    observer.OnNext(new OrderSent(order));
                                }
                                Update(order).GetAwaiter().GetResult();
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

        private FileSystemWatcher DataPathWatcher
        {
            get;
        }

        private string FileName
        {
            get;
        }

        private string FullFilePath
        {
            get;
        }

        public bool SaveData()
        {
            RepositoryDTO dto = new RepositoryDTO();
            lock (ClientLock)
            {
                dto.Clients = ClientManager.GetAll().Select(c => new ClientDTO(c)).ToList();
            }
            lock (ProductLock)
            {
                dto.Products = ProductManager.GetAll().Select(p => new ProductDTO(p)).ToList();
            }
            lock (OrderLock)
            {
                dto.Orders = OrderManager.GetAll().Select(o => new OrderDTO(o)).ToList();
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
                                ClientManager.ReplaceData(new HashSet<IClient>(dto.Clients.Select(c => c.ToIClient())));
                                OrderManager.ReplaceData(new HashSet<IOrder>(dto.Orders.Select(o => o.ToIOrder())));
                                ProductManager.ReplaceData(new HashSet<IProduct>(dto.Products.Select(p => p.ToIProduct())));
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

        public Task<IClient> CreateClient(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            lock (ClientLock)
            {
                string id;
                try
                {
                    id = ClientManager.Create(username, firstName, lastName, street, streetNumber, phoneNumber);
                }
                catch (Exception)
                {
                    return null;
                }
                SaveData();
                return Task.FromResult(ClientManager.Get(id));
            }
        }

        public Task<IOrder> CreateOrder(string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, DateTime? deliveryDate)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(clientUsername) == null)
                {
                    return null;
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in productIdQuantityMap)
                    {
                        IProduct product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return null;
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    lock (OrderLock)
                    {
                        uint id;
                        try
                        {
                            id = OrderManager.Create(clientUsername, orderDate, productIdQuantityMap, totalPrice,
                                deliveryDate);
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                        SaveData();
                        return Task.FromResult(OrderManager.Get(id));
                    }
                }
            }
        }

        public Task<IProduct> CreateProduct(string name, double price, ProductType productType)
        {
            lock (ProductLock)
            {
                uint id;
                try
                {
                    id = ProductManager.Create(name, price, productType);
                }
                catch (Exception)
                {
                    return null;
                }
                SaveData();
                return Task.FromResult(ProductManager.Get(id));
            }
        }

        public Task<bool> OpenRepository()
        {
            if (System.IO.File.Exists(FullFilePath))
            {
                LoadData();
            }
            DataPathWatcher.Changed += DataFile_Changed;
            DataPathWatcher.Deleted += DataFile_Deleted;
            DataPathWatcher.EnableRaisingEvents = true;
            return Task.FromResult(true);
        }

        public Task<HashSet<IClient>> GetAllClients()
        {
            lock (ClientLock)
            {
                return Task.FromResult(ClientManager.GetAll());
            }
        }

        public Task<HashSet<IOrder>> GetAllOrders()
        {
            lock (OrderLock)
            {
                return Task.FromResult(OrderManager.GetAll());
            }
        }

        public Task<HashSet<IProduct>> GetAllProducts()
        {
            lock (ProductLock)
            {
                return Task.FromResult(ProductManager.GetAll());
            }
        }

        public Task<IClient> GetClient(string username)
        {
            lock (ClientLock)
            {
                return Task.FromResult(ClientManager.Get(username));
            }
        }

        public Task<IOrder> GetOrder(uint id)
        {
            lock (OrderLock)
            {
                return Task.FromResult(OrderManager.Get(id));
            }
        }

        public Task<IProduct> GetProduct(uint id)
        {
            lock (ProductLock)
            {
                return Task.FromResult(ProductManager.Get(id));
            }
        }

        public Task<bool> RemoveClient(IClient client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    if (ClientManager.Get(client.Username) == null)
                    {
                        return Task.FromResult(false);
                    }

                    bool changed = false;
                    foreach (uint orderId in OrderManager.GetAll().Where(order => order.ClientUsername == client.Username).Select(o => o.Id))
                    {
                        changed |= OrderManager.Remove(orderId);
                    }

                    if (ClientManager.Remove(client.Username))
                    {
                        SaveData();
                        return Task.FromResult(true);
                    }

                    if (changed)
                    {
                        SaveData();
                    }
                    return Task.FromResult(false);
                }
            }
        }

        public Task<bool> RemoveOrder(IOrder order)
        {
            lock (OrderLock)
            {
                if (OrderManager.Remove(order.Id))
                {
                    SaveData();
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
        }

        public Task<bool> RemoveProduct(IProduct product)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    if (ProductManager.Get(product.Id) == null)
                    {
                        return Task.FromResult(false);
                    }
                    foreach (IOrder order in OrderManager.GetAll())
                    {
                        if (order.ProductIdQuantityMap.ContainsKey(product.Id))
                        {
                            return Task.FromResult(false);
                        }
                    }

                    if (ProductManager.Remove(product.Id))
                    {
                        SaveData();
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
            }
        }

        public Task<bool> Update(IClient client)
        {
            lock (ClientLock)
            {
                lock (OrderLock)
                {
                    if (ClientManager.Update(client))
                    {
                        SaveData();
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                }
            }
        }

        public Task<bool> Update(IOrder order)
        {
            lock (ClientLock)
            {
                if (ClientManager.Get(order.ClientUsername) == null)
                {
                    return Task.FromResult(false);
                }
                lock (ProductLock)
                {
                    double totalPrice = 0.0;
                    foreach (KeyValuePair<uint, uint> pair in order.ProductIdQuantityMap)
                    {
                        IProduct product = ProductManager.Get(pair.Key);
                        if (product == null)
                        {
                            return Task.FromResult(false);
                        }
                        totalPrice += product.Price * pair.Value;
                    }
                    order.Price = totalPrice;
                    lock (OrderLock)
                    {
                        if (OrderManager.Update(order))
                        {
                            SaveData();
                            return Task.FromResult(true);
                        }

                        return Task.FromResult(false);
                    }
                }
            }
        }

        public Task<bool> UpdateClient(string username, string firstName, string lastName, string street, uint streetNumber,
            string phoneNumber)
        {
            return Update(new Client(username, firstName, lastName, street, streetNumber, phoneNumber));
        }

        public Task<bool> UpdateOrder(uint id, string clientUsername, DateTime orderDate, Dictionary<uint, uint> productIdQuantityMap, double price,
            DateTime? deliveryDate)
        {
            return Update(new Order(id, clientUsername, orderDate, productIdQuantityMap, price, deliveryDate));
        }

        public Task<bool> UpdateProduct(uint id, string name, double price, ProductType productType)
        {
            return Update(new Product(id, name, price, productType));
        }

        public Task<bool> Update(IProduct product)
        {
            lock (ProductLock)
            {
                lock (OrderLock)
                {
                    if (ProductManager.Update(product))
                    {
                        SaveData();
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
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

        public IDisposable Subscribe(IObserver<DataChanged<IClient>> observer)
        {
            return ClientManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IProduct>> observer)
        {
            return ProductManager.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<DataChanged<IOrder>> observer)
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
